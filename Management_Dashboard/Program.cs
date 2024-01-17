using Management_Dashboard.Filters;
using Management_Dashboard.Services;
using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
string baseUrl = builder.Configuration["ApiBaseUrl"];
// Inject Application Settings 
var applicationSettings = builder.Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
builder.Services.Configure<ApplicationSettings>(options => builder.Configuration.GetSection(nameof(ApplicationSettings)).Bind(options));
builder.Services.AddSingleton<ApplicationSettings>(x => x.GetRequiredService<IOptions<ApplicationSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(new MongoClient(applicationSettings!.DatabaseSettings.ConnectionString));

builder.Services.Configure<SQLConnectionStrings>(builder.Configuration.GetSection(nameof(SQLConnectionStrings)));
builder.Services.AddSingleton<SQLConnectionStrings>(x => x.GetRequiredService<IOptions<SQLConnectionStrings>>().Value);

builder.Services.AddDbContext<PostgreSQLDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnectionString")));

//MongoDB Configuration
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
})
.AddDefaultTokenProviders()
.AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(applicationSettings!.DatabaseSettings.ConnectionString, applicationSettings.DatabaseSettings.Database);


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Management_Dashboard", Version = "v1" });

    // Include XML comments for Swagger generation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);


    //Include Base Url
    c.DocumentFilter<BasePathDocumentFilter>(baseUrl);
});

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UsePathBase("/" + baseUrl);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();   
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

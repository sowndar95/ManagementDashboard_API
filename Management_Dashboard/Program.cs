using Management_Dashboard.Services;
using ManagementDashboard_Entities;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Inject Application Settings 
var applicationSettings = builder.Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
builder.Services.Configure<ApplicationSettings>(options => builder.Configuration.GetSection(nameof(ApplicationSettings)).Bind(options));
builder.Services.AddSingleton<ApplicationSettings>(x => x.GetRequiredService<IOptions<ApplicationSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(new MongoClient(applicationSettings!.DatabaseSettings.ConnectionString));
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
});

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

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

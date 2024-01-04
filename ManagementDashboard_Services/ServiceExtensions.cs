using ManagementDashboard_Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Management_Dashboard.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection service, ConfigurationManager configuration)
        {
            //service.AddDataProtection();

            service.AddScoped<UserProfileService>();
            service.AddScoped<TimeSheetService>();
            service.AddScoped<CustomerService>();
            service.AddScoped<ProjectService>();

            return service;
        }
    }
}

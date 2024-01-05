using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Services
{
    public sealed class ProjectService : BaseService<Project>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectService(ApplicationSettings settings, UserManager<ApplicationUser> userManager) : base(settings)
        {
            _userManager = userManager;
        }

        public async Task<List<Project>> GetProjects(Guid customerId)
        {
            var result = await base.Find(f => f.Customer == customerId);
            return result.ToList();
        }

        public async Task<List<Customer>> GetCustomersByProjectId(List<Guid> projectIds)
        {
            List<Customer> lstCustomer = new List<Customer>();
            var orgId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.OrganizationId);

            lstCustomer = (await base.Find(x => x.Organization == orgId && projectIds.Contains(x.Id)))
                .Select(s => new Customer()
                {
                    Id = s.Customer,
                    CustomerName = s.CustomerName,
                }).ToList();
            return lstCustomer;
        }
    }
}

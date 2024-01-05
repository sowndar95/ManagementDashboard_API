using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Services
{
    public sealed class CustomerService : BaseService<Customer>
    {
        private readonly UserProfileService _userProfileService;
        private readonly ProjectService _projectService;
        public CustomerService(ApplicationSettings settings, UserManager<ApplicationUser> userManager, UserProfileService userProfileService, ProjectService projectService) : base(settings)
        {
            _userProfileService = userProfileService;
            _projectService = projectService;
        }


        public async Task<List<Customer>> GetCustomerListByManagerId(Guid userId)
        {
            // Get Project details from UserProfile table passing Userid. Then pass the ProjectId into Project table and get Customer details.
            List<Customer> lstCustomer = new List<Customer>();
            List<Project> lstProject = new List<Project>();
            var userProfileInfo = await _userProfileService.GetEmployeeById(userId);
            if (userProfileInfo != null && userProfileInfo.Projects != null && userProfileInfo.Projects.Count > 0)
            {
                lstCustomer = await _projectService.GetCustomersByProjectId(userProfileInfo.Projects);
                //foreach (var item in userProfileInfo.Projects)
                //{
                //    lstProject = await _projectService.GetProjectsByProjectId(item);
                //    if(lstProject != null) 
                //    {
                //        lstCustomer = lstProject.Select(x => new Customer()
                //        {
                //            Id = x.Customer,
                //            CustomerName = x.CustomerName
                //        }).ToList();
                //    }
                //}                
            }
            lstCustomer = lstCustomer.DistinctBy(c => c.Id).ToList();
            return lstCustomer;
        }
    }
}

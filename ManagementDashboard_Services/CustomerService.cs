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
        private readonly TimeSheetService _timeSheetService;

        public CustomerService(ApplicationSettings settings, UserManager<ApplicationUser> userManager, UserProfileService userProfileService, ProjectService projectService, TimeSheetService timeSheetService) : base(settings)
        {
            _userProfileService = userProfileService;
            _projectService = projectService;
            _timeSheetService = timeSheetService;
        }

        public async Task<List<Customer>> GetClientListByManagerId(Guid userId, DateTime fromDate, Guid orgId)
        {
            var result = await _timeSheetService.GetClientListByManagerId(userId, fromDate, orgId);

            return result;
        }
    }
}

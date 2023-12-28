using Management_Dashboard.Controllers;
using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Mvc;

namespace Management_Dashboard.Controllers
{
    public class DashboardController : BaseController<UserProfile>
    {

        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger, UserProfileService service) : base(service)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all Employee Details.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IList<UserProfile>>> GetAllUsers()
        {
            var result = await ((UserProfileService)service).GetAll();
            return result.ToList();
        }

        /// <summary>
        /// Get all Employees under Manager.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetEmployeesListByManager(Guid id)
        {
            var result = await ((UserProfileService)service).GetEmployeesByManager(id);
            return result;
        }

        /// <summary>
        /// Get all Employees by Name.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetEmployeeByName(string name)
        {
            var result = await ((UserProfileService)service).GetEmployeeByName(name);
            return result;
        }
    }
}
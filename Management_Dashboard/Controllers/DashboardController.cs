using Management_Dashboard.Controllers;
using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Management_Dashboard.Controllers
{
    public class DashboardController : BaseController<UserProfile>
    {

        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger, UserProfileService service ) : base(service)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get Manager Hierarchy.
        /// </summary>
        /// <remarks>
        /// This method is used to retrive hierarchy of Managers who are reporting to that provided ManagerID
        /// </remarks>
        /// <param name="id">The unique identifier of the manager.</param>
        /// <returns>Returns a hierarchy of Managers who are reporting to that provided ManagerID.</returns>
        /// <response code="200">Returns the list of Managers.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        public async Task<ActionResult<IList<UserProfile>>> GetManagerHierarchy(Guid id)
        {
            var result = await ((UserProfileService)service).GetManagerHierarchy(id);
            return result.ToList();
        }

        /// <summary>
        /// Get all Employees under Manager.
        /// </summary>
        /// <remarks>
        /// This method is used to retrive Employees who are reporting to that provided ManagerID.
        /// </remarks>
        /// <param name="id">The unique identifier of the manager.</param>
        /// <returns>Returns a list of Employees who are reporting to that provided ManagerID.</returns>
        /// <response code="200">Returns the list of Employees.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        public async Task<ActionResult<IList<UserProfile>>> GetEmployeesListByManager(Guid id)
        {
            var result = await ((UserProfileService)service).GetEmployeesListByManager(id);
            return result.ToList();
        }      
    }
}
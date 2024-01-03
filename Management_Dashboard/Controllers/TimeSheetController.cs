using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Mvc;

namespace Management_Dashboard.Controllers
{
    public class TimeSheetController : BaseController<TimeSheet>
    {

        private readonly ILogger<TimeSheetController> _logger;

        public TimeSheetController(ILogger<TimeSheetController> logger, TimeSheetService service) : base(service)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all Employee Details.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IList<TimesheetModel>>> GetTimeSheetEntries(Guid mangerId, DateTime fromDate, DateTime toDate, string option)
        {
            var result = await ((TimeSheetService)service).GetEmployeeTimesheetEntries(mangerId, fromDate, toDate, option);
            return result.ToList();
        }
    }
}

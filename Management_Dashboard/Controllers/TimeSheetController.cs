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
        public async Task<ActionResult<IList<TimesheetModel>>> GetTimeSheetEntries(Guid mangerId, DateTime fromDate, int interval, int mode, string option, int timeoff, Guid projectId)
        {
            var result = await ((TimeSheetService)service).GetEmployeeTimesheetEntries(mangerId, fromDate, interval, mode, option, timeoff, projectId);
            return result.ToList();
        }
    }
}

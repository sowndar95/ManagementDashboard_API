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
        /// Get Employees Productivity report.
        /// </summary>
        /// <remarks>
        /// This method is used to retrive Employees Productivity Report Managers who are reporting to the provided ManagerID
        /// </remarks>
        /// <param name="mangerId">The unique identifier of the manager.</param>
        /// <param name="fromDate">Date from which the report generated.</param>
        /// <param name="interval">Date Range (Ex: 5 => 5 months report from FromDate).</param>
        /// <param name="mode">Type of Report (0 => Weekly or 1 => Monthly).</param>
        /// <response code="200">Returns Employees Productivity Report.</response>
        [HttpGet]
        public async Task<ActionResult<IList<TimesheetModel>>> GetTimeSheetEntries(Guid mangerId, DateTime fromDate, int interval, int mode, string option, Guid projectId)
        {
            var result = await ((TimeSheetService)service).GetEmployeeTimesheetEntries(mangerId, fromDate, interval, mode, option, projectId);
            return result.ToList();
        }
    }
}

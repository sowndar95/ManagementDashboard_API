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
        public async Task<ActionResult<IList<TimesheetModel>>> GetEmployeeProductivity(Guid mangerId, DateTime fromDate, int interval, int mode, string option, Guid projectId)
        {
            var result = await ((TimeSheetService)service).GetEmployeeProductivity(mangerId, fromDate, interval, mode, option, projectId);
            return result.ToList();
        }

        /// <summary>
        /// Get Non-Split Entries Report.
        /// </summary>
        /// <remarks>
        /// This method is used to retrive Non-Split Entries Report of the provided ManagerID
        /// </remarks>
        /// <param name="mangerId">The unique identifier of the manager.</param>
        /// <param name="fromDate">Date from which the report generated.</param>
        /// <param name="entryCount">Entry Count (Ex: 2,3,4).</param>
        /// <response code="200">Returns Non-Split Entries Report.</response>
        [HttpGet]
        public async Task<ActionResult<IList<TimesheetModel>>> GetNonSplitTimesheetEntriesByUser(Guid mangerId, DateTime fromDate, int entryCount)
        {
            var result = await ((TimeSheetService)service).GetNonSplitTimesheetEntriesByUser(mangerId, fromDate, entryCount);
            return result.ToList();
        }

        /// <summary>
        /// Get Non-Split Entries Report by User by Weekly.
        /// </summary>
        /// <remarks>
        /// This method is used to retrive Non-Split Entries Weekly Report of the provided ManagerID and UserID
        /// </remarks>
        /// <param name="mangerId">The unique identifier of the manager.</param>
        /// <param name="fromDate">Date from which the report generated.</param>
        /// <param name="entryCount">Entry Count (Ex: 2,3,4).</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <response code="200">Returns Non-Split Entries Weekly Report.</response>
        [HttpGet]
        public async Task<ActionResult<IList<NonSplitEntry>>> GetNonSplitTimesheetEntriesByUserByWeekly(Guid mangerId, DateTime fromDate, int entryCount, Guid userId)
        {
            var result = await ((TimeSheetService)service).GetNonSplitTimesheetEntriesByUserByWeekly(mangerId, fromDate, entryCount, userId);
            return result.ToList();
        }

        /// <summary>
        /// Get TimeSheet Approval Report by Manager by Weekly.
        /// </summary>
        /// <remarks>
        /// This method is used to retrive TimeSheet Approval Report of the provided ManagerID
        /// </remarks>
        /// <param name="mangerId">The unique identifier of the manager.</param>
        /// <param name="fromDate">Date from which the report generated.</param>
        /// <param name="toDate">Date till which the report generated.</param>
        /// <response code="200">Returns TimeSheet Approval by Weekly Report.</response>
        [HttpGet]
        public async Task<ActionResult<IList<TimeSheetApproval>>> GetTimesheetApprovalStatus(Guid mangerId, DateTime fromDate, DateTime toDate)
        {
            var result = await ((TimeSheetService)service).GetTimesheetApprovalStatus(mangerId, fromDate, toDate);
            return result.ToList();
        }        
    }
}

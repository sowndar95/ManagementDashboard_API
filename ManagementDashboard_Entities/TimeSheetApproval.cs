using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class TimeSheetApproval
    {
        public string Name { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string ProjectTask { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string TimesheetStatus { set; get; } = string.Empty;
        public List<ProjectApprovalStatus> listProjectApprovalStatus { get; set; }
    }

    public class ProjectApprovalStatus
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemValue { get; set; } = string.Empty;

    }
}

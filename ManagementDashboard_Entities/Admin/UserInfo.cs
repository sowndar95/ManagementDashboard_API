using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class UserInfo
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Mobile { get; set; }
        public string ReportingManagerCode { get; set; }
        public string ReportingManagerName { get; set; }
        public string ReportingManagerEmail { get; set; }
        public bool IsLoginEnabled { get; set; }
        public int AccessLevel { get; set; }
        public int UserId { get; set; }
        public string Customer { get; set; }
        public string Photo { get; set; }
        public bool IsGroup { get; set; }
        public int OrganizationId { get; set; }
        public List<UserWidgetInfo> userWidgetInfo { get; set; }
    }

    public class UserWidgetInfo
    {
        public int WidgetId { get; set; }
        public string WidgetName { get; set; }
        public bool Level { get; set; }

    }
}

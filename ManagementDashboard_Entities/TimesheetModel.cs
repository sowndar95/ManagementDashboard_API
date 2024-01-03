using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public sealed class TimesheetModel
    {
        public string MonthYrPart { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Hours { get; set; }
        public int Count { get; set; }
        public decimal NonProductivity { get; set; }
        public string Percentage { get; set; } = string.Empty;
        public int Project { get; set; }
        public DateTime Date { get; set; }
        public string WeeklyStart { get; set; } = string.Empty;
        public decimal TimeOffHours { get; set; }
        public double WorkingDays { get; set; }
        public decimal ProductivityHours { get; set; }
    }
}

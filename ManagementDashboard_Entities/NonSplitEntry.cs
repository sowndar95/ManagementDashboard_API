using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class NonSplitEntry : ManagementDashboardEntityBase
    {
        public string MonthYrPart { get; set; } = string.Empty;
        public Guid UserId { get; set; } 
        public string UserCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Hours { get; set; }
        public int Count { get; set; }
        public decimal NonProductivity { get; set; }
        public string Percentage { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string WeeklyStart { get; set; } = string.Empty;
        public decimal TimeOffHours { get; set; }
        public int? WorkingDays { get; set; }
        public decimal ProductivityHours { get; set; }
        public List<NonSplitDrillDonw> nonSplitDrillDonws { get; set; }
    }

    public class NonSplitDrillDonw
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

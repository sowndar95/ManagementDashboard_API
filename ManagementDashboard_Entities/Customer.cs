using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Customer: ManagementDashboardEntityBase
    {
        public string CustomerName { get; set; } = null!;
        public bool? IsBillable { get; set; }
        public decimal? BudgetedWeeklyHours { get; set; }
        public decimal? BudgetedMonthlyHours { get; set; }
        public decimal? BudgetedTotalHours { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }
}

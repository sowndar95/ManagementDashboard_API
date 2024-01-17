using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_Customer
    {
        [Key]
        public int ProjectId { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public decimal? WeeklyBudgetedHours { get; set; }
        public decimal? MonthlyBudgetedHours { get; set; }
        public decimal? TotalBudgetedHours { get; set; }
        public bool? IsBillable { get; set; }
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public bool? IsReferenceMandatory { get; set; }
        public bool? IsRencataManagerApproval { get; set; }
        public bool? ShowRencataTimeOffToClient { get; set; }
    }
}

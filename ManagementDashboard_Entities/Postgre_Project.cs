using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_Project
    {
        [Key]
        public int ProjectTaskId { get; set; }
        public string Name { get; set; }
        public int? IsActive { get; set; }
        public int ProjectId { get; set; }
        public decimal? BudgetedHours { get; set; }
        public int? IsBillable { get; set; }
        public int? ProjectPlaceHolderId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? IsPaid { get; set; }
        public int? IsProductivity { get; set; }
    }
}

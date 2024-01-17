using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_ProjectAllocation
    {
        [Key]
        public int ProjectTaskAllocationId { get; set; }
        public int UserId { get; set; }
        public int ManagerId { get; set; }
        public int ProjectTaskId { get; set; }
        public decimal AllocationPercentage { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}

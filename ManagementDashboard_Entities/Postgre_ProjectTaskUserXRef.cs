using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_ProjectTaskUserXRef
    {
        [Key]
        public int ProjectTaskUserXRefId { get; set; }
        public int ProjectTaskId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsBillable { get; set; }
        public int PayType { get; set; }
        public decimal PayRate { get; set; }
        public decimal BillRate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public int ClientApproverId { get; set; }
        public bool ClientApproverIsActive { get; set; }
        public int DesignationId { get; set; }
        public int CommissionTypeCodeId { get; set; }
        public decimal CommissionValue { get; set; }
        public decimal ProjectAllocationPercentage { get; set; }
    }
}

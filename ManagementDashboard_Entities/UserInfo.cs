using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_UserInfo
    {
        [Key]
        public int Id { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public string? Designation { get; set; }
        public string? Level { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Mobile { get; set; }
        public DateTime? DOB { get; set; }
        public string? Team { get; set; }
        public string? ReportingManagerCode { get; set; }
        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerEmail { get; set; }
        public DateTime? DOJ { get; set; }
        public string? Photo { get; set; }
        public DateTime? LWD { get; set; }
        public DateTime? EffectiveDateOfReporting { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool? Active { get; set; }
        public string? Platinum { get; set; }
        public string? Gold { get; set; }
        public string? Silver { get; set; }
        public string? BloodGroup { get; set; }
        public string? UPN { get; set; }
        public string? ItemType { get; set; }
        public string? Path { get; set; }
        public bool? IsLoginEnabled { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? AccessLevel { get; set; }
        public bool? IsGroup { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_UserOrganizationXRef
    {
        public int UserOrganizationXRefId { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int DesignationId { get; set; }
        public bool IsBoxedLayout { get; set; }
        public int SkinTypeId { get; set; }
        public string Reason { get; set; }
        public string PlacedRoleTypes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public int StateId { get; set; }
        public string DepartmentCodes { get; set; }
        public bool IsTimesheetHistory { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public int ContractorId { get; set; }
        public bool IsStaffing { get; set; }
        public bool IsTimesheetApprovalHistory { get; set; }
        public bool IsOverTimeApplicable { get; set; }
        public bool IsQuickTimeSheetSubmit { get; set; }
        public string ClientAssociateId { get; set; }
        public bool IsOverTimeShowUser { get; set; }
        public bool IsOverTimeShowManager { get; set; }
        public bool IsOverTimeShowClientApprover { get; set; }
    }
}

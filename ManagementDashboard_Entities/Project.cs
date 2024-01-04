using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Project: ManagementDashboardEntityBase
    {
        public Guid Organization { get; set; }
        public Guid Customer { get; set; }

        public string? CustomerName { get; set; }
        public string EarnCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime ExpiryDate { get; set; }
        [BsonIgnore]
        public AssignUserToProject AssignedUser { get; set; } = new();
        public ManagerInformation PrimaryApprover { get; set; } = new();
        public List<ManagerInformation> SecondaryApprovers { get; set; } = new();
        public int BudgetedHour { get; set; } = new();
        public List<Lock> ProjectLockInfo { get; set; } = new();
        public List<Allocation> Allocations { get; set; } = new();
    }

    public sealed class AssignUserToProject
    {
        public Guid Project { get; set; }
        public List<AssignUserStatus> UserStatus { get; set; } = new();
        //public string EarnCodeId { get; set; } = string.Empty;
    }
    public sealed class AssignUserStatus
    {
        public Guid User { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }

    public sealed class Allocation
    {
        public Guid User { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }
        [BsonIgnore]
        public string UserFullName { get; set; }
    }
}

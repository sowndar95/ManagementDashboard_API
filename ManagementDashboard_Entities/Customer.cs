using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Customer: ManagementDashboardEntityBase
    {
        public Guid Organization { get; set; }
        public string CustomerName { get; set; } = null!;
        public bool? IsBillable { get; set; }
        public decimal? BudgetedWeeklyHours { get; set; }
        public decimal? BudgetedMonthlyHours { get; set; }
        public decimal? BudgetedTotalHours { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }

        public List<ManagerInformation> ResourceManagers { get; set; } = new();
        public List<ManagerInformation> ClientApprovers { get; set; } = new();
        public List<ManagerInformation> PrimryClientApprovers { get; set; } = new();
        public List<ManagerInformation> SecondryClientApprovers { get; set; } = new();
        public List<Lock> LockInfo { get; set; } = new();
        public List<CustomerNotification> NotificationInfo { get; set; } = new();
        public List<Departments> Departments { get; set; } = new();
        public List<CommonDetails> Designation { get; set; } = new();

        [BsonIgnore]
        public string ResourceManagerNames
        {
            get
            {
                if (ResourceManagers.Count == 0) return string.Empty;
                return string.Join(",", ResourceManagers.Select(s => s.FullName));
            }
        }

        [BsonIgnore]
        public string ClientApproversNames
        {
            get
            {
                if (ClientApprovers.Count == 0) return string.Empty;
                return string.Join(",", ClientApprovers.Select(s => s.FullName));
            }
        }

        [BsonIgnore]
        public string PrimryClientApproversNames
        {
            get
            {
                if (PrimryClientApprovers.Count == 0) return string.Empty;
                return string.Join(",", PrimryClientApprovers.Select(s => s.FullName));
            }
        }
        [BsonIgnore]
        public string SecondryClientApproversNames
        {
            get
            {
                if (SecondryClientApprovers.Count == 0) return string.Empty;
                return string.Join(",", SecondryClientApprovers.Select(s => s.FullName));
            }
        }
    }
    public sealed class ManagerInformation
    {
        public Guid User { get; set; }
        public string? FullName { get; set; }
    }

    public sealed class Lock
    {
        public bool IsLocked { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

        [BsonRepresentation(BsonType.Document)]
        public DateTimeOffset UpdatedDate { get; set; }

        public Guid UpdatedBy { get; set; }
    }

    public sealed class CustomerNotification
    {

        public string Report { get; set; }
        public bool IsChecked { get; set; } = false;
    }

    public sealed class Departments
    {
        public Guid Code { get; set; }
        public string Description { get; set; } = null!;
        public List<DepartmentCode> DepartmentCode { get; set; } = new();
    }

    public sealed class DepartmentCode
    {
        public Guid Code { get; set; }
        public string Description { get; set; } = null!;
        public ManagerInformation PrimaryApprover { get; set; } = new();
        public List<ManagerInformation> SecondaryApprovers { get; set; } = new();
    }

    public sealed class CommonDetails
    {
        public Guid Code { get; set; }
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }

}

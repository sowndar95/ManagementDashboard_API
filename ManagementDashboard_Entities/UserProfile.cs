using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ManagementDashboard_Entities;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace ManagementDashboard_Entities;
public sealed class UserProfile : ManagementDashboardEntityBase
{
    public Guid Organization { get; set; }
    public Guid User { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserCode { get; set; } = null!;
    public DateTime? JoiningDate { get; set; }
    public DateTime? LastWorkingDate { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = string.Empty;
    public Guid? Designation { get; set; }
    public Guid? OrganizationId { get; set; }

    #region Status
    public void ActivateUser(bool isActivate)
    {
        EmailConfirmed = isActivate;
        DeActivated = !isActivate;
        LastWorkingDate = !isActivate ? DateTime.Now : null;
    }

    public void LockOut(DateTimeOffset? date)
    {
        LockoutEnd = date;
    }

    public bool DeActivated { get; set; } = false;
    public bool EmailConfirmed { get; set; } = false;
    public DateTimeOffset? LockoutEnd { get; set; } = null;
    public string Status
    {
        get
        {
            if (DeActivated)
            {
                return "DeActive";
            }
            if (!EmailConfirmed)
            {
                return "NotConfirmed";
            }
            if (LockoutEnd.HasValue)
            {
                return "Locked";
            }
            return "Active";
        }
        private set { }
    }

    #endregion

    [JsonIgnore]
    public string UserProfileImage { get; set; } = string.Empty;
    public int? DayMinHours { get; set; }
    public int? WeekMinHours { get; set; }
    public string EmploymentType { get; set; } = string.Empty;

    [BsonIgnore]
    public bool IsPunchTimeUser
    {
        get
        {
            return EmploymentType == "PunchTypeUser";
        }
    }


    public Guid? ManagerId { get; set; }
    public string ClientAssociateId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsStaffing { get; set; }
    public bool IsTimesheetExempt { get; set; }
    public bool IsOverTime { get; set; }
    public Address Address { get; set; } = new();
    public List<Guid> Projects { get; set; } = new();
    public List<DesignationInformation> DesignationInformation { get; set; } = new();
    public List<ReportSetting> ReportSettings { get; set; } = new();



    [BsonIgnore]
    public string FullName
    {
        get
        {
            string returnValue = $"{FirstName} {LastName}";
            if (!string.IsNullOrWhiteSpace(UserCode))
                returnValue = $"{returnValue} - {UserCode}";
            return returnValue;
        }
    }

    [BsonIgnore]
    public string FullNameWithoutCode { get { return $"{FirstName} {LastName}"; } }

    [BsonIgnore]
    public string RoleNames { get; set; } = string.Empty;

    [BsonIgnore]
    public List<Guid> RoleIds { get; set; } = new();

    [BsonIgnore]
    public bool IsSystemUser
    {
        get
        {
            if (RoleNames.Contains("Organization Admin")) return true;
            if (RoleNames.Contains("Maintenance Admin")) return true;

            return false;
        }
    }


    [BsonIgnore]
    public string ManagerName { get; set; } = string.Empty;
}

public sealed class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int? ZipCode { get; set; }
    public string Country { get; set; } = string.Empty;

}

public sealed class DesignationInformation
{
    public Guid Customer { get; set; }

    public Guid Designation { get; set; }

    public Guid Department { get; set; }

    public Guid DepartmentCode { get; set; }
}

public sealed class ReportSetting
{
    public string Report { get; set; } = string.Empty;

    public List<ReportColumn> ReportColumns { get; set; } = new();
}

public sealed class ReportColumn
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool Show { get; set; }
}


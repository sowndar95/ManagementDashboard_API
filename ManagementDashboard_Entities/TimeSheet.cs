using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public sealed class TimeSheet : ManagementDashboardEntityBase
    {
        [JsonIgnore]
        public Guid Organization { get; set; }
        public Guid Customer { get; set; }
        public Guid Project { get; set; }
        public Guid User { get; set; }
        public DateTime Date { get; set; }
        public DateTimeOffset TimeSheetEnteredDate { get; set; }

        public int Hours { get; set; }
        public int Minutes { get; set; }

        [BsonIgnore]
        public string FormattedTime
        {
            get
            {
                return CustomTime.ConvertTimeToDisplayFormat(Hours, Minutes);
            }
        }

        public string Reference { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Week { get; set; }

        public string Status { get; set; } = "Created";

        [BsonIgnore]
        public string OrganizationName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public Guid? SubmittedBy { get; set; }
        public DateTimeOffset? SubmittedDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTimeOffset? ApprovedDate { get; set; }
        public Guid? RejectedBy { get; set; }
        public DateTimeOffset? RejectedDate { get; set; }
        public string? RejectedComments { get; set; }

        public string TimesheetLegend { get; set; } = string.Empty;
        public string TimesheetSubmittedLegend { get; set; } = string.Empty;
        public string? TimesheetApprovalMode { get; set; }

        public Guid? EnteredBy { get; set; }

        #region Earn Codes
        public int? Regular { get; set; }
        public int? OverTime { get; set; }
        public int? DoubleTime { get; set; }

        [JsonIgnore]
        public int DailyTotalHours { get; set; }
        [JsonIgnore]
        public int DailyTotalMinutes { get; set; }
        [JsonIgnore]
        public int WeeklyTotalHours { get; set; }
        [JsonIgnore]
        public int WeeklyTotalMinutes { get; set; }
        [JsonIgnore]
        public int WeeklyRegularTotalMinutes { get; set; }
        #endregion

    }
}

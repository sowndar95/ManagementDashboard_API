using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class UserInfoByManager
    {
        public int ProjectTaskAllocationId { get; set; }
        public int UserId { get; set; }
        public int ManagerId { get; set; }
        public int ProjectId { get; set; }
        public int ProjectTaskId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string ClientName { get; set; }
        public string Customer { get; set; }
        public string ProjectName { get; set; }
        public string ReportingManagerCode { get; set; }
        public string ReportingManagerName { get; set; }
        public string AllocationPercentage { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ClientApproverName { get; set; }
    }

    public class UserProjectInfo
    {
        public List<CustomerInfo> listCustomer { get; set; }
        public List<ProjectInfo> listProject { get; set; }

    }


    public class CustomerInfo
    {
        public int ProjectId { get; set; }
        public string Customer { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class ProjectInfo
    {
        public int? ProjectId { get; set; }
        public string ClientName { get; set; }
        public int? ProjectTaskId { get; set; }
        public string ProjectName { get; set; }
        public bool ProjectStatus { get; set; }
        public int UserId { get; set; }
    }

    public class ProjectTaskAllocation
    {
        public int? ProjectTaskAllocationId { get; set; }
        public int? UserId { get; set; }
        public int ProjectTaskId { get; set; }
        public decimal AllocationPercentage { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserIds { get; set; }
        public int ManagerId { get; set; }
    }

    public class WidgetInfo
    {
        public int WidgetId { get; set; }
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string SectionName { get; set; }
        public string WidgetName { get; set; }
        public bool Level2 { get; set; }
        public bool Level3 { get; set; }
        public string Description { get; set; }
    }

    public class AllocatedInfo
    {
        public string Allocated { get; set; }
        public string Message { get; set; }
    }
}

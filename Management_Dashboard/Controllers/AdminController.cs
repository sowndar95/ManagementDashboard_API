using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using MongoDB.Driver.Core.Configuration;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.NetworkInformation;

namespace Management_Dashboard.Controllers
{
    public class AdminController : BaseController<Project>
    {

        private readonly ILogger<AdminController> _logger;
        private readonly PostgreSQLDbContext _dbContext;
        public AdminController(ILogger<AdminController> logger, AdminService service, PostgreSQLDbContext postgreSQLDbContext) : base(service)
        {
            _logger = logger;
            _dbContext = postgreSQLDbContext;
        }

        //[HttpGet]
        //public async Task<ActionResult<IList<UserInfo>>> GetAuthorizationByManagedUser(string empCode)
        //{
        //    List<UserInfo> ReportList = new List<UserInfo> { };
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);

        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetAuthorizationByManagedUser_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@EmployeeCode", empCode));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();

        //                while (sqlReader.Read())
        //                {
        //                    ReportList.Add(new UserInfo
        //                    {
        //                        EmployeeCode = sqlReader["EmployeeCode"].ToString(),
        //                        EmployeeName = sqlReader["EmployeeName"].ToString(),
        //                        Email = sqlReader["Email"].ToString(),
        //                        Designation = sqlReader["Designation"].ToString(),
        //                        Department = sqlReader["Department"].ToString(),
        //                        Location = sqlReader["Location"].ToString(),
        //                        Mobile = sqlReader["Mobile"].ToString(),
        //                        ReportingManagerCode = sqlReader["ReportingManagerCode"].ToString(),
        //                        ReportingManagerName = sqlReader["ReportingManagerName"].ToString(),
        //                        ReportingManagerEmail = sqlReader["ReportingManagerEmail"].ToString(),
        //                        IsLoginEnabled = Convert.ToBoolean(sqlReader["IsLoginEnabled"].ToString()),
        //                        AccessLevel = Convert.ToInt16(sqlReader["AccessLevel"].ToString()),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportList;
        //}

        [HttpPost]
        public async Task<ActionResult<string>> GetEmployeeByEmployeeCode(string employeeCode)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();

            try
            {
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("PostgreSQLConnectionString").ToString()))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(@"SELECT public.get_employee_by_empcode(:p_EmployeeCode)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("p_EmployeeCode", employeeCode);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var employeeJson = reader.GetValue(0).ToString();
                                return Ok(employeeJson);
                            }
                            else
                            {
                                return NotFound($"Employee with ID {employeeCode} not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> UpdateUserAccessLevel(string empCode, int accesslvl)
        {            
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            try
            {
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("PostgreSQLConnectionString").ToString()))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(@"SELECT updateuseraccesslevel(
	                                                                :p_empCode,
                                                                    :p_AccessLevel
                                                                )", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("p_empCode", empCode);
                        command.Parameters.AddWithValue("p_AccessLevel", accesslvl);
                        command.ExecuteNonQuery();
                        return Ok("Function executed successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); ;
            }           
        }

        [HttpPost]
        public async Task<ActionResult<string>> UpdateAuthorizationByManagedUser(string employeeCode)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();

            try
            {
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("PostgreSQLConnectionString").ToString()))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(@"SELECT UpdateAuthorizationByManagedUser(:p_EmployeeCode)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("p_EmployeeCode", employeeCode);
                        command.ExecuteNonQuery();
                        return Ok("Function executed successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        //[HttpGet]
        //public async Task<ActionResult<List<UserInfo>>> GetAuthorizedManager(string employeeCode)
        //{
        //    List<UserInfo> ReportList = new List<UserInfo> { };
        //    ReportList = GetAutherizedManagerInfo(employeeCode);
        //    foreach (var item in ReportList)
        //    {
        //        List<UserWidgetInfo> lstwidget = new List<UserWidgetInfo>();
        //        lstwidget = GetWidgetInfoList(item.AccessLevel);
        //        item.userWidgetInfo = lstwidget;
        //    }
        //    return ReportList;
        //}

        //public List<UserInfo> GetAutherizedManagerInfo(string employeeCode)
        //{
        //    List<UserInfo> ReportList = new List<UserInfo> { };
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetAuthorizedManager_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@EmployeeCode", employeeCode));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportList.Add(new UserInfo
        //                    {
        //                        UserId = Convert.ToInt16(sqlReader["UserId"].ToString()),
        //                        EmployeeCode = sqlReader["EmployeeCode"].ToString(),
        //                        EmployeeName = sqlReader["EmployeeName"].ToString(),
        //                        Email = sqlReader["Email"].ToString(),
        //                        Location = sqlReader["Location"].ToString(),
        //                        Mobile = sqlReader["Mobile"].ToString(),
        //                        ReportingManagerCode = sqlReader["ReportingManagerCode"].ToString(),
        //                        ReportingManagerName = sqlReader["ReportingManagerName"].ToString(),
        //                        Photo = sqlReader["Photo"].ToString(),
        //                        IsLoginEnabled = Convert.ToBoolean(sqlReader["IsLoginEnabled"].ToString()),
        //                        AccessLevel = Convert.ToInt16(sqlReader["AccessLevel"].ToString()),
        //                        IsGroup = Convert.ToBoolean(sqlReader["IsGroup"].ToString()),
        //                        OrganizationId = Convert.ToInt16(sqlReader["OrganizationId"].ToString()),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportList;
        //}

        //public List<UserWidgetInfo> GetWidgetInfoList(int level)
        //{
        //    List<UserWidgetInfo> ReportList = new List<UserWidgetInfo> { };
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetMobileWidgetByUserAccessLevel_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@AccessLevel", level));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportList.Add(new UserWidgetInfo
        //                    {
        //                        WidgetId = Convert.ToInt16(sqlReader["WidgetId"].ToString()),
        //                        WidgetName = sqlReader["WidgetName"].ToString(),
        //                        Level = Convert.ToBoolean(sqlReader["LevelAcess"].ToString()),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportList;
        //}

        //[HttpGet]
        //public List<UserInfoByManager> GetUsersDetailsByManager(DateTime? effectiveDate = null, DateTime? endDate = null)
        //{

        //    List<UserInfoByManager> ReportList = new List<UserInfoByManager> { };
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetUsersDetailsByManager_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@EffectiveDate", effectiveDate));
        //                cmd.Parameters.Add(new SqlParameter("@EndDate", endDate));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportList.Add(new UserInfoByManager
        //                    {
        //                        ProjectTaskAllocationId = Convert.ToInt32(sqlReader["ProjectTaskAllocationId"].ToString()),
        //                        UserId = Convert.ToInt32(sqlReader["UserId"].ToString()),
        //                        ManagerId = Convert.ToInt32(sqlReader["ManagerId"].ToString()),
        //                        ProjectId = Convert.ToInt32(sqlReader["ProjectId"].ToString()),
        //                        ProjectTaskId = Convert.ToInt32(sqlReader["ProjectTaskId"].ToString()),
        //                        EmployeeCode = sqlReader["EmployeeCode"].ToString(),
        //                        EmployeeName = sqlReader["EmployeeName"].ToString(),
        //                        Customer = sqlReader["CustomerName"].ToString(),
        //                        ProjectName = sqlReader["ProjectName"].ToString(),
        //                        ClientName = sqlReader["ClientName"].ToString(),
        //                        ReportingManagerName = sqlReader["ReportingManagerName"].ToString(),
        //                        AllocationPercentage = !string.IsNullOrEmpty(sqlReader["AllocationPercentage"].ToString()) ? string.Format("{0}{1}", sqlReader["AllocationPercentage"].ToString(), "%") : "No Allocation",
        //                        EffectiveDate = Convert.ToDateTime(sqlReader["EffectiveDate"].ToString()),
        //                        EndDate = Convert.ToDateTime(sqlReader["EndDate"].ToString()),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportList;
        //}

        //[HttpGet]
        //public UserProjectInfo GetCustomerAndProjectByUserId(int userId)
        //{
        //    UserProjectInfo ReportListUser = new UserProjectInfo { };
        //    List<ProjectInfo> ReportListProject = new List<ProjectInfo> { };
        //    List<CustomerInfo> ReportListCustomer = new List<CustomerInfo> { };

        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetCustomerAndProjectByUserId_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@UserId", userId));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportListProject.Add(new ProjectInfo
        //                    {
        //                        UserId = Convert.ToInt32(sqlReader["UserId"].ToString()),
        //                        ProjectTaskId = Convert.ToInt32(sqlReader["ProjectTaskId"].ToString()),
        //                        ProjectName = sqlReader["Name"].ToString(),
        //                        ProjectId = Convert.ToInt32(sqlReader["ProjectId"].ToString()),
        //                    });
        //                }
        //                if (sqlReader.NextResult())
        //                {
        //                    while (sqlReader.Read())
        //                    {
        //                        ReportListCustomer.Add(new CustomerInfo
        //                        {
        //                            UserId = Convert.ToInt32(sqlReader["UserId"].ToString()),
        //                            OrganizationId = Convert.ToInt32(sqlReader["OrganizationId"].ToString()),
        //                            Customer = sqlReader["Name"].ToString(),
        //                            ProjectId = Convert.ToInt32(sqlReader["ProjectId"].ToString()),
        //                        });
        //                    }
        //                }
        //            }
        //            ReportListUser.listProject = ReportListProject;
        //            ReportListUser.listCustomer = ReportListCustomer;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportListUser;
        //}

        //[HttpPost]
        //public string InsertProjectAllocationByUser(ProjectTaskAllocation projectTaskAllocation)
        //{
        //    string status = string.Empty;
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_InsertProjectAllocationByUser_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@ProjectTaskAllocationId", projectTaskAllocation.ProjectTaskAllocationId));
        //                cmd.Parameters.Add(new SqlParameter("@ProjectTaskId", projectTaskAllocation.ProjectTaskId));
        //                cmd.Parameters.Add(new SqlParameter("@UserId", projectTaskAllocation.UserId));
        //                cmd.Parameters.Add(new SqlParameter("@ManagerId", projectTaskAllocation.ManagerId));
        //                cmd.Parameters.Add(new SqlParameter("@AllocationPercentage", projectTaskAllocation.AllocationPercentage));
        //                cmd.Parameters.Add(new SqlParameter("@EffectiveDate", projectTaskAllocation.EffectiveDate));
        //                cmd.Parameters.Add(new SqlParameter("@EndDate", projectTaskAllocation.EndDate));
        //                SqlParameter returnParameter = cmd.Parameters.Add("@Result", SqlDbType.Int);
        //                returnParameter.Direction = ParameterDirection.Output;
        //                cmd.ExecuteNonQuery();
        //                int Output = (int)returnParameter.Value;
        //                if (Output >= 1)
        //                    status = "Inserted Successfully";
        //                else
        //                    status = "Error while updating";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return status;
        //}

        //[HttpPost]
        //public string DeleteProjectAllocation(int ProjectTaskAllocationId)
        //{
        //    string status = string.Empty;
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_DeleteProjectAllocation_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@ProjectTaskAllocationId", ProjectTaskAllocationId));
        //                SqlParameter returnParameter = cmd.Parameters.Add("@Result", SqlDbType.Int);
        //                returnParameter.Direction = ParameterDirection.Output;
        //                cmd.ExecuteNonQuery();
        //                int Output = (int)returnParameter.Value;
        //                if (Output >= 1)
        //                    status = "Deleted Successfully";
        //                else
        //                    status = "Error while updating";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return status;
        //}

        //[HttpGet]
        //public List<WidgetInfo> GetMobileWidgetByAccessLevel(int accessLevel)
        //{
        //    List<WidgetInfo> ReportWidgetInfo = new List<WidgetInfo> { };
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetMobileWidgetByAccessLevel_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@AccessLevel", accessLevel));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportWidgetInfo.Add(new WidgetInfo
        //                    {
        //                        WidgetId = Convert.ToInt32(sqlReader["WidgetId"].ToString()),
        //                        ScreenId = Convert.ToInt32(sqlReader["ScreenId"].ToString()),
        //                        WidgetName = sqlReader["WidgetName"].ToString(),
        //                        ScreenName = sqlReader["ScreenName"].ToString(),
        //                        Level2 = Convert.ToBoolean(sqlReader["Level 2"].ToString()),
        //                        Level3 = Convert.ToBoolean(sqlReader["Level 3"].ToString()),
        //                        Description = sqlReader["Description"].ToString(),
        //                        SectionName = sqlReader["SectionName"].ToString(),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportWidgetInfo;
        //}

        //[HttpPost]
        //public bool UpdateMobileWidgetByAccessLevel(List<WidgetInfo> reportWidgetInfo)
        //{
        //    System.Data.DataTable dtWidget = new System.Data.DataTable();
        //    dtWidget = ToDataTable(reportWidgetInfo);
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_UpdateMobileWidgetByAccessLevel_MgmtApp]", connection))
        //            {
        //                SqlDataAdapter sqlDatAdapter = new SqlDataAdapter(cmd);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@WidgetId", SqlDbType.Int, 8, "WidgetId");
        //                cmd.Parameters.Add("@ScreenId", SqlDbType.Int, 8, "ScreenId");
        //                //cmd.Parameters.Add("@IsActive", SqlDbType.Int, 8, "IsActive");
        //                cmd.Parameters.Add("@Level2", SqlDbType.BigInt, 2, "Level2");
        //                cmd.Parameters.Add("@Level3", SqlDbType.BigInt, 2, "Level3");
        //                sqlDatAdapter.InsertCommand = cmd;
        //                sqlDatAdapter.Update(dtWidget);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return true;
        //}

        //[NonAction]
        //public System.Data.DataTable ToDataTable<T>(List<T> items)
        //{
        //    System.Data.DataTable dataTable = new System.Data.DataTable(typeof(T).Name);
        //    //Get all the properties
        //    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo prop in Props)
        //    {
        //        //Setting column names as Property names
        //        switch (prop.Name)
        //        {
        //            case "WidgetId":
        //                dataTable.Columns.Add(prop.Name, typeof(int));
        //                break;
        //            case "ScreenId":
        //                dataTable.Columns.Add(prop.Name, typeof(int));
        //                break;
        //            case "Level2":
        //                dataTable.Columns.Add(prop.Name, typeof(bool));
        //                break;
        //            case "Level3":
        //                dataTable.Columns.Add(prop.Name, typeof(bool));
        //                break;
        //            default:
        //                dataTable.Columns.Add(prop.Name);
        //                break;
        //        }
        //    }
        //    foreach (T item in items)
        //    {
        //        var values = new object[Props.Length];
        //        for (int i = 0; i < Props.Length; i++)
        //        {
        //            //inserting property values to datatable rows
        //            values[i] = Props[i].GetValue(item, null);
        //        }
        //        dataTable.Rows.Add(values);
        //    }
        //    //put a breakpoint here and check datatable
        //    return dataTable;
        //}

        //[HttpPost]
        //public List<AllocatedInfo> ValidateEmployeeProjectAllocationBetweenDates(ProjectTaskAllocation projectTaskAllocation)
        //{
        //    List<AllocatedInfo> ReportAllocatedInfo = new List<AllocatedInfo> { };

        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_ValidateEmployeeProjectAllocationBetweenDates_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@UserId", projectTaskAllocation.UserId));
        //                cmd.Parameters.Add(new SqlParameter("@EffectiveDate", projectTaskAllocation.EffectiveDate));
        //                cmd.Parameters.Add(new SqlParameter("@EndDate", projectTaskAllocation.EndDate));
        //                cmd.Parameters.Add(new SqlParameter("@ProjectTaskId", projectTaskAllocation.ProjectTaskId));
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportAllocatedInfo.Add(new AllocatedInfo
        //                    {
        //                        Allocated = sqlReader["Allocated"].ToString(),
        //                        Message = sqlReader["Message"].ToString(),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportAllocatedInfo;
        //}

        //[HttpGet]
        //public List<UserInfoByManager> GetAllUsersDetails()
        //{
        //    List<UserInfoByManager> ReportList = new List<UserInfoByManager> { };
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json", optional: false);
        //    var configuration = builder.Build();
        //    using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("[STimesheet].[up_GetAllUsersDetails_MgmtApp]", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                SqlDataReader sqlReader = cmd.ExecuteReader();
        //                while (sqlReader.Read())
        //                {
        //                    ReportList.Add(new UserInfoByManager
        //                    {
        //                        UserId = Convert.ToInt32(sqlReader["UserId"].ToString()),
        //                        ManagerId = Convert.ToInt32(sqlReader["ManagerId"].ToString()),
        //                        EmployeeCode = sqlReader["EmployeeCode"].ToString(),
        //                        EmployeeName = sqlReader["EmployeeName"].ToString(),
        //                        ReportingManagerCode = sqlReader["ReportingManagerCode"].ToString(),
        //                        ReportingManagerName = sqlReader["ReportingManagerName"].ToString(),
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
        //        }
        //    }
        //    return ReportList;
        //}

    }
}

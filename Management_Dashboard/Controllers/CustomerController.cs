using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Management_Dashboard.Controllers
{
    public class CustomerController : BaseController<Customer>
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly PostgreSQLDbContext _dbContext;
        public CustomerController(ILogger<CustomerController> logger, CustomerService service, PostgreSQLDbContext postgreSQLDbContext ) : base(service)
        {
            _logger = logger;
            _dbContext = postgreSQLDbContext;
        }

        /// <summary>
        /// Get Customer Details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fromDate"></param>
        /// <param name="orgId"></param>
        /// <returns>Return Customer id and Customer Name</returns>
        [HttpGet]
        public async Task<ActionResult<IList<Customer>>> GetClientList(Guid userId, DateTime fromDate, Guid orgId)
        {
            var result = await ((CustomerService)service).GetClientListByManagerId(userId, fromDate, orgId);
            return result.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<List<Postgre_Customer>>> GetCustomerList()
        {
            List<Postgre_Customer> postgre_Customer = new();
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false);

                var configuration = builder.Build();
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM [SAdmin].[Project]"; 

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Postgre_Customer customer = new Postgre_Customer
                                    {
                                        ProjectId = (int)reader["ProjectId"],
                                        OrganizationId = (int)reader["OrganizationId"],
                                        Name = reader["Name"].ToString(),
                                        IsActive = (bool)reader["IsActive"],
                                        WeeklyBudgetedHours = reader["WeeklyBudgetedHours"] != DBNull.Value ? (decimal)reader["WeeklyBudgetedHours"] : (decimal?)null,
                                        MonthlyBudgetedHours = reader["MonthlyBudgetedHours"] != DBNull.Value ? (decimal)reader["MonthlyBudgetedHours"] : (decimal?)null,
                                        TotalBudgetedHours = reader["TotalBudgetedHours"] != DBNull.Value ? (decimal)reader["TotalBudgetedHours"] : (decimal?)null,
                                        IsBillable = reader["IsBillable"] != DBNull.Value ? (bool)reader["IsBillable"] : (bool?)null,
                                        ClientId = reader["ClientId"] != DBNull.Value ? (int)reader["ClientId"] : (int?)null,
                                        ClientName = reader["ClientName"] != DBNull.Value ? reader["ClientName"].ToString() : null,
                                        CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : null,
                                        CreatedDate = DateTime.SpecifyKind((DateTime)reader["CreatedDate"], DateTimeKind.Utc),
                                        ModifiedDate = DateTime.SpecifyKind((DateTime)reader["ModifiedDate"], DateTimeKind.Utc),
                                        EffectiveDate = reader["EffectiveDate"] != DBNull.Value ? DateTime.SpecifyKind((DateTime)reader["EffectiveDate"], DateTimeKind.Utc) : (DateTime?)null,
                                        ExpiredDate = reader["ExpiredDate"] != DBNull.Value ? DateTime.SpecifyKind((DateTime)reader["ExpiredDate"], DateTimeKind.Utc) : (DateTime?)null,
                                        ModifiedBy = reader["ModifiedBy"] != DBNull.Value ? reader["ModifiedBy"].ToString() : null,                                        
                                        IsReferenceMandatory = reader["IsReferenceMandatory"] != DBNull.Value ? (bool)reader["IsReferenceMandatory"] : (bool?)null,
                                        IsRencataManagerApproval = reader["IsRencataManagerApproval"] != DBNull.Value ? (bool)reader["IsRencataManagerApproval"] : (bool?)null,
                                        ShowRencataTimeOffToClient = reader["ShowRencataTimeOffToClient"] != DBNull.Value ? (bool)reader["ShowRencataTimeOffToClient"] : (bool?)null
                                    };
                                    await AddCustomer(customer);
                                    postgre_Customer.Add(customer);
                                }
                                reader.Close();
                            }
                            else
                            {
                                Console.WriteLine("No records found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }

            return postgre_Customer;
        }

        [HttpGet]
        public async Task<ActionResult<List<Postgre_Project>>> GetProjectsList()
        {
            List<Postgre_Project> postgre_Project = new();
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false);

                var configuration = builder.Build();
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM [SAdmin].[ProjectTask]";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Postgre_Project project = new Postgre_Project
                                    {
                                        ProjectTaskId = (int)reader["ProjectTaskId"],
                                        Name = reader["Name"].ToString(),
                                        IsActive = reader["IsActive"] != DBNull.Value ? (bool)reader["IsActive"] ? 1 : 0 : null,
                                        ProjectId = (int)reader["ProjectId"],
                                        BudgetedHours = reader["BudgetedHours"] != DBNull.Value ? (decimal)reader["BudgetedHours"] : (decimal?)null,
                                        IsBillable = reader["IsBillable"] != DBNull.Value ? (bool)reader["IsActive"] ? 1 : 0 : null,
                                        ProjectPlaceHolderId = reader["ProjectPlaceHolderId"] != DBNull.Value ? (int)reader["ProjectPlaceHolderId"] : null,
                                        CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : null,
                                        CreatedDate = DateTime.SpecifyKind((DateTime)reader["CreatedDate"], DateTimeKind.Utc),
                                        ModifiedDate = DateTime.SpecifyKind((DateTime)reader["ModifiedDate"], DateTimeKind.Utc),
                                        EffectiveDate = reader["EffectiveDate"] != DBNull.Value ? DateTime.SpecifyKind((DateTime)reader["EffectiveDate"], DateTimeKind.Utc) : (DateTime?)null,
                                        ExpiredDate = reader["ExpiredDate"] != DBNull.Value ? DateTime.SpecifyKind((DateTime)reader["ExpiredDate"], DateTimeKind.Utc) : (DateTime?)null,
                                        ModifiedBy = reader["ModifiedBy"] != DBNull.Value ? reader["ModifiedBy"].ToString() : null,
                                        IsPaid = reader["IsPaid"] != DBNull.Value ? (bool)reader["IsActive"] ? 1 : 0 : null,
                                        IsProductivity = reader["IsProductivity"] != DBNull.Value ? (bool)reader["IsActive"] ? 1 : 0 : null,

                                    };
                                    await AddProject(project);
                                    postgre_Project.Add(project);
                                }
                                reader.Close();
                            }
                            else
                            {
                                Console.WriteLine("No records found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }
            var count = postgre_Project.Count();
            return postgre_Project;
        }

        [HttpGet]
        public async Task<ActionResult<List<Postgre_UserInfo>>> GetUserInfo()
        {
            List<Postgre_UserInfo> postgre_User = new();
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false);

                var configuration = builder.Build();
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM [dbo].[UserInfo_MgmtApp]";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Postgre_UserInfo user = new Postgre_UserInfo
                                    {
                                        EmployeeCode = reader["Employee Code"] != DBNull.Value ? reader["Employee Code"].ToString() : null,
                                        EmployeeName = reader["Employee Name"] != DBNull.Value ? reader["Employee Name"].ToString() : null,
                                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                                        Designation = reader["Designation"] != DBNull.Value ? reader["Designation"].ToString() : null,
                                        Level = reader["Level"] != DBNull.Value ? reader["Level"].ToString() : null,
                                        Department = reader["Department"] != DBNull.Value ? reader["Department"].ToString() : null,
                                        Location = reader["Location"] != DBNull.Value ? reader["Location"].ToString() : null,
                                        Mobile = reader["Mobile"] != DBNull.Value ? reader["Mobile"].ToString() : null,
                                        DOB = reader["DOB"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["DOB"], DateTimeKind.Utc) : null,
                                        Team = reader["Team"] != DBNull.Value ? reader["Team"].ToString() : null,
                                        ReportingManagerCode = reader["Reporting Manager Code"] != DBNull.Value ? reader["Reporting Manager Code"].ToString() : null,
                                        ReportingManagerName = reader["Reporting Manager Name"] != DBNull.Value ? reader["Reporting Manager Name"].ToString() : null,
                                        ReportingManagerEmail = reader["Reporting Manager Email"] != DBNull.Value ? reader["Reporting Manager Email"].ToString() : null,
                                        DOJ = reader["DOJ"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["DOJ"], DateTimeKind.Utc) : null,
                                        Photo = reader["Photo"] != DBNull.Value ? reader["Photo"].ToString() : null,
                                        LWD = reader["LWD"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["LWD"], DateTimeKind.Utc) : null,
                                        EffectiveDateOfReporting = reader["Effective Date Of Reporting"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Effective Date Of Reporting"], DateTimeKind.Utc) : null,
                                        Modified = reader["Modified"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Modified"], DateTimeKind.Utc) : null,
                                        CreatedDate = reader["Created Date"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Created Date"], DateTimeKind.Utc) : null,
                                        CreatedBy = reader["Created By"] != DBNull.Value ? reader["Created By"].ToString() : null,
                                        ModifiedBy = reader["Modified By"] != DBNull.Value ? reader["Modified By"].ToString() : null,
                                        Active = reader["Active"] != DBNull.Value ? (bool?)reader["Active"] : null,
                                        Platinum = reader["Platinum"] != DBNull.Value ? reader["Platinum"].ToString() : null,
                                        Gold = reader["Gold"] != DBNull.Value ? reader["Gold"].ToString() : null,
                                        Silver = reader["Silver"] != DBNull.Value ? reader["Silver"].ToString() : null,
                                        BloodGroup = reader["Blood Group"] != DBNull.Value ? reader["Blood Group"].ToString() : null,
                                        UPN = reader["UPN"] != DBNull.Value ? reader["UPN"].ToString() : null,
                                        ItemType = reader["Item Type"] != DBNull.Value ? reader["Item Type"].ToString() : null,
                                        Path = reader["Path"] != DBNull.Value ? reader["Path"].ToString() : null,
                                        IsLoginEnabled = reader["IsLoginEnabled"] != DBNull.Value ? (bool?)reader["IsLoginEnabled"] : null,
                                        ModifiedDate = reader["Modified Date"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Modified Date"], DateTimeKind.Utc) : null,
                                        IsDeleted = reader["IsDeleted"] != DBNull.Value ? (bool?)reader["IsDeleted"] : null,
                                        AccessLevel = reader["AccessLevel"] != DBNull.Value ? (int)reader["AccessLevel"] : null,
                                        IsGroup = reader["IsGroup"] != DBNull.Value ? (bool?)reader["IsGroup"] : null,

                                    };
                                    await AddUser(user);
                                    postgre_User.Add(user);
                                }
                                reader.Close();
                            }
                            else
                            {
                                Console.WriteLine("No records found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }
            var count = postgre_User.Count();
            return postgre_User;
        }

        [HttpGet]
        public async Task<ActionResult<List<Postgre_UserInfo>>> GetProjectAllocation()
        {
            List<Postgre_UserInfo> postgre_User = new();
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false);

                var configuration = builder.Build();
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("SQLConnectionString").ToString()))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM [dbo].[UserInfo_MgmtApp]";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Postgre_UserInfo user = new Postgre_UserInfo
                                    {
                                        EmployeeCode = reader["Employee Code"] != DBNull.Value ? reader["Employee Code"].ToString() : null,
                                        EmployeeName = reader["Employee Name"] != DBNull.Value ? reader["Employee Name"].ToString() : null,
                                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                                        Designation = reader["Designation"] != DBNull.Value ? reader["Designation"].ToString() : null,
                                        Level = reader["Level"] != DBNull.Value ? reader["Level"].ToString() : null,
                                        Department = reader["Department"] != DBNull.Value ? reader["Department"].ToString() : null,
                                        Location = reader["Location"] != DBNull.Value ? reader["Location"].ToString() : null,
                                        Mobile = reader["Mobile"] != DBNull.Value ? reader["Mobile"].ToString() : null,
                                        DOB = reader["DOB"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["DOB"], DateTimeKind.Utc) : null,
                                        Team = reader["Team"] != DBNull.Value ? reader["Team"].ToString() : null,
                                        ReportingManagerCode = reader["Reporting Manager Code"] != DBNull.Value ? reader["Reporting Manager Code"].ToString() : null,
                                        ReportingManagerName = reader["Reporting Manager Name"] != DBNull.Value ? reader["Reporting Manager Name"].ToString() : null,
                                        ReportingManagerEmail = reader["Reporting Manager Email"] != DBNull.Value ? reader["Reporting Manager Email"].ToString() : null,
                                        DOJ = reader["DOJ"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["DOJ"], DateTimeKind.Utc) : null,
                                        Photo = reader["Photo"] != DBNull.Value ? reader["Photo"].ToString() : null,
                                        LWD = reader["LWD"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["LWD"], DateTimeKind.Utc) : null,
                                        EffectiveDateOfReporting = reader["Effective Date Of Reporting"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Effective Date Of Reporting"], DateTimeKind.Utc) : null,
                                        Modified = reader["Modified"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Modified"], DateTimeKind.Utc) : null,
                                        CreatedDate = reader["Created Date"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Created Date"], DateTimeKind.Utc) : null,
                                        CreatedBy = reader["Created By"] != DBNull.Value ? reader["Created By"].ToString() : null,
                                        ModifiedBy = reader["Modified By"] != DBNull.Value ? reader["Modified By"].ToString() : null,
                                        Active = reader["Active"] != DBNull.Value ? (bool?)reader["Active"] : null,
                                        Platinum = reader["Platinum"] != DBNull.Value ? reader["Platinum"].ToString() : null,
                                        Gold = reader["Gold"] != DBNull.Value ? reader["Gold"].ToString() : null,
                                        Silver = reader["Silver"] != DBNull.Value ? reader["Silver"].ToString() : null,
                                        BloodGroup = reader["Blood Group"] != DBNull.Value ? reader["Blood Group"].ToString() : null,
                                        UPN = reader["UPN"] != DBNull.Value ? reader["UPN"].ToString() : null,
                                        ItemType = reader["Item Type"] != DBNull.Value ? reader["Item Type"].ToString() : null,
                                        Path = reader["Path"] != DBNull.Value ? reader["Path"].ToString() : null,
                                        IsLoginEnabled = reader["IsLoginEnabled"] != DBNull.Value ? (bool?)reader["IsLoginEnabled"] : null,
                                        ModifiedDate = reader["Modified Date"] != DBNull.Value ? (DateTime?)DateTime.SpecifyKind((DateTime)reader["Modified Date"], DateTimeKind.Utc) : null,
                                        IsDeleted = reader["IsDeleted"] != DBNull.Value ? (bool?)reader["IsDeleted"] : null,
                                        AccessLevel = reader["AccessLevel"] != DBNull.Value ? (int)reader["AccessLevel"] : null,
                                        IsGroup = reader["IsGroup"] != DBNull.Value ? (bool?)reader["IsGroup"] : null,

                                    };
                                    await AddUser(user);
                                    postgre_User.Add(user);
                                }
                                reader.Close();
                            }
                            else
                            {
                                Console.WriteLine("No records found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }
            var count = postgre_User.Count();
            return postgre_User;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCustomers()
        {
            var customers = await _dbContext.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCustomerById(int id)
        {
            var customer = await _dbContext.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Postgre_Customer>> AddCustomer([FromBody] Postgre_Customer newCustomer)
        {
            if (newCustomer == null)
            {
                return BadRequest("Invalid customer data.");
            }
            try
            {
                var insertedCustomer = await _dbContext.InsertCustomersAsync(newCustomer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }
           

            return newCustomer;
        }

        [HttpPost]
        public async Task<ActionResult<Postgre_Project>> AddProject([FromBody] Postgre_Project newProject)
        {
            if (newProject == null)
            {
                return BadRequest("Invalid customer data.");
            }
            try
            {
                var insertedCustomer = await _dbContext.InsertProjectAsync(newProject);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }
            return newProject;
        }

        [HttpPost]
        public async Task<ActionResult<Postgre_UserInfo>> AddUser([FromBody] Postgre_UserInfo newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid customer data.");
            }
            try
            {
                var insertedCustomer = await _dbContext.InsertUserAsync(newUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data from SQL: " + ex.Message);
            }
            return newUser;
        }
    }
}

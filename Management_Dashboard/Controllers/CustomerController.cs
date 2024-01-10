using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Management_Dashboard.Controllers
{
    public class CustomerController : BaseController<Customer>
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger, CustomerService service ) : base(service)
        {
            _logger = logger;
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

    }
}

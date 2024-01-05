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
        /// <returns>Return Customer id and Customer Name</returns>
        [HttpGet]
        public async Task<ActionResult<IList<Customer>>> GetCustomerListByManagerId(Guid userId)
        {
            var result = await ((CustomerService)service).GetCustomerListByManagerId(userId);
            return result.ToList();
        }


    }
}

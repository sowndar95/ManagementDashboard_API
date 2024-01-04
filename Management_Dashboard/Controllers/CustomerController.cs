using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Management_Dashboard.Controllers
{
    public class CustomerController : BaseController<Customer>
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger, CustomerService service) : base(service)
        {
            _logger = logger;
        }


    }
}

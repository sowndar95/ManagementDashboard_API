using ManagementDashboard_Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Services
{
    public sealed class CustomerService : BaseService<Customer>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerService(ApplicationSettings settings, UserManager<ApplicationUser> userManager) : base(settings)
        {
            _userManager = userManager;
        }


    }
}

using ManagementDashboard_Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Services
{
    public sealed class AdminService : BaseService<Project>
    {
        public AdminService(ApplicationSettings settings) : base(settings)
        {
        }

    }
}

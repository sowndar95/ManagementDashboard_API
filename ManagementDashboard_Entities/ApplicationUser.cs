using AspNetCore.Identity.MongoDbCore.Models;
using ManagementDashboard_Entities.Base;
using Microsoft.VisualBasic;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Guid OrganizationId { get; set; }
        public bool DeActivated { get; set; } = false;

        public string Status
        {
            get
            {
                if (DeActivated)
                {
                    return EnumConstants.UserStatuses.DeActive;
                }
                if (!EmailConfirmed)
                {
                    return EnumConstants.UserStatuses.NotConfirmed;
                }
                if (LockoutEnd.HasValue)
                {
                    return EnumConstants.UserStatuses.Locked;
                }
                return EnumConstants.UserStatuses.Active;
            }
            private set { }
        }
    }
}

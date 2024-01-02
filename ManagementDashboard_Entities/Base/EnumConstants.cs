using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities.Base
{
    public class EnumConstants
    {
        public static UserStatus UserStatuses { get; set; } = new();
        public sealed class UserStatus
        {
            public string NotConfirmed => "Not Confirmed";
            public string Active => "Active";
            public string DeActive => "De-Active";
            public string Locked => "Locked";
        }
    }
}

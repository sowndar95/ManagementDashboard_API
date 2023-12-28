using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public sealed class ApplicationSettings
    {
        public DatabaseSettings DatabaseSettings { get; set; } = new();
    }

    public sealed class DatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
    }
}

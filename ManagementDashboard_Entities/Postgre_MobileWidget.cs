using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public class Postgre_MobileWidget
    {
        public int WidgetId { get; set; }
        public int ScreenId { get; set; }
        public string SectionName { get; set; }
        public string WidgetName { get; set; }
        public bool Level2 { get; set; }
        public bool Level3 { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
    }
}

using ManagementDashboard_Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Services

{
    public sealed class UserProfileService : BaseService<UserProfile>
    {
        public UserProfileService(ApplicationSettings settings) : base(settings) { }

        public override async Task<IList<UserProfile>> GetAll()
        {
            var result = await base.GetAll();
            return result;
        }

        public async Task<UserProfile> GetEmployeesByManager(Guid managerId)
        {
            var result = await base.Find(managerId);          
            return result;
        }

        public async Task<UserProfile> GetEmployeeByName(string name)
        {
            Expression<Func<UserProfile, bool>> expression = u => u.FirstName == name;
            var result = await base.FindOne(expression);
            return result;
        }

        
    }
}

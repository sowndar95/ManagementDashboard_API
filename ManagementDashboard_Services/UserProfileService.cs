using ManagementDashboard_Entities;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ManagementDashboard_Entities.Base.EnumConstants;

namespace ManagementDashboard_Services

{
    public sealed class UserProfileService : BaseService<UserProfile>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserProfileService(ApplicationSettings settings, UserManager<ApplicationUser> userManager) : base(settings)
        {
            _userManager = userManager;
        }

        public override async Task<IList<UserProfile>> GetAll()
        {
            var result = await base.GetAll();
            return result;
        }

        public async Task<IList<UserProfile>> GetEmployeesByManager(Guid id)
        {
            var orgId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.OrganizationId);
            var roleId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.ManagerRoleId);
            var userStatus = ManagementDashboard_Utilites.Common.AppConstants.Status;

            List<UserProfile> lstEmployee = new List<UserProfile>();
            List<Guid> userIds = new List<Guid>();

            var result = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Status == userStatus).ToList();

            if (result == null)
            {
                return new List<UserProfile>();
            }
            return lstEmployee;
        }

        public async Task<IList<UserProfile>> GetManagerHierarchy1(Guid managerId)
        {
            var orgId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.OrganizationId);
            var roleId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.ManagerRoleId);
            var userStatus = ManagementDashboard_Utilites.Common.AppConstants.Status;

            //var result = (await base.GetAll()).Where(x => x.ManagerId == id).ToList();

            //List<UserProfile> lstManager = new List<UserProfile>();
            //var result = await base.GetAll();
            //var topManagers = result.Where(x => x.ManagerId == id).ToList();
            //if (result.Count > 0)
            //{                
            //    lstManager = topManagers.ToList();
            //    lstManager.Insert(0, result.Where(x => x.User == id).FirstOrDefault());
            //    //lstManager.Insert(0, new UserProfile() { Id = id });
            //    //var subManagers = result.Select(x => x.User).Except(topManagers.Select(y => y.User)).ToList();
            //    var secondLevel = result.Except(topManagers).ToList();
            //    var result1 = secondLevel.Where(a => lstManager.Any(b => a.ManagerId == b.User));

            //    var subManagers = result1.Where(x => secondLevel.Any(x => x.User == x.ManagerId));
            //}

            //return lstManager;


            List<Guid> userIds = new List<Guid>();

            var applicationUsers = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Status == userStatus).ToList();

            if (applicationUsers == null)
            {
                return new List<UserProfile>();
            }

            var userList = applicationUsers.Where(x => x.Roles.Contains(roleId)).ToList();

            if (userList != null && userList.Any())
            {
                var ids = userList.Select(x => x.Id).Distinct().ToList();
                userIds.AddRange(ids);
            }

            if (userIds == null || !userIds.Any())
            {
                return new List<UserProfile>();
            }

            userIds = userIds.Distinct().ToList();

            var result = (await Find(f => f.Organization == orgId
                                        && userIds.Contains(f.User)))
                         .Select(s => new UserProfile()
                         {
                             User = s.Id,
                             UserCode = s.UserCode,
                             FirstName = s.FirstName,
                             LastName = s.LastName,
                         }).ToList();

            return result;
        }

        public async Task<IList<UserProfile>> GetManagerHierarchy(Guid managerId)
        {
            List<UserProfile> lstManagers = new List<UserProfile>();
            var orgId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.OrganizationId);
            var roleId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.ManagerRoleId);
            var userStatus = ManagementDashboard_Utilites.Common.AppConstants.Status;

            var applicationUsers = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Status == userStatus).ToList();

            if (applicationUsers == null || applicationUsers.Count == 0)
            {
                return new List<UserProfile>();
            }

            var userList = applicationUsers.Where(x => x.Roles.Contains(roleId)).ToList();

            if (userList != null)
            {
                var users = await base.GetAll();
                var topManager = users.Where(x => x.User == managerId).ToList();

                if (topManager != null)
                {
                    var id = topManager.Select(x => x.User).SingleOrDefault();
                    var userIds = await GetSubManagerIds(users, id);

                    if (userIds != null)
                    {
                        lstManagers = (await Find(f => f.Organization == orgId
                                       && userIds.Contains(f.User)))
                        .Select(s => new UserProfile()
                        {
                            User = s.Id,
                            UserCode = s.UserCode,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                        }).ToList();
                        
                    }
                }
            }

             return lstManagers;
        }

        private async Task<List<Guid>> GetSubManagerIds(IList<UserProfile> users, Guid managerId)
        {
            List<Guid> userIds = new List<Guid>();
            userIds.Add(managerId);
            bool flag = false;
            if (users == null)
            {
                return new List<Guid>();
            }

            foreach (var item in users)
            {
                if (item.ManagerId == managerId)
                {
                    var guid = item.User;
                    if (users.Where(x => x.ManagerId == guid).Any())
                    {
                        userIds.Add(item.User);
                    }
                }
            }

            return userIds;
        }
        public async Task<List<UserProfile>> GetEmployeesListByManager(Guid id)
        {
            List<UserProfile> lstEmployees = new List<UserProfile>();
            var result = await base.GetAll();
            lstEmployees = result.Where(x => x.ManagerId == id).ToList();

            return lstEmployees;
        }

        public async Task<UserProfile> GetEmployeeByName(string name)
        {
            Expression<Func<UserProfile, bool>> expression = u => u.FirstName == name;
            var result = await base.FindOne(expression);
            return result;
        }
    }
}

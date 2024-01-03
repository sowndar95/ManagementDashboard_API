using ManagementDashboard_Entities;
using ManagementDashboard_Utilites.Common;
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
            List<Guid> userIds = new();

            var result = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Status == userStatus).ToList();

            if (result == null)
            {
                return new List<UserProfile>();
            }
            return lstEmployee;
        }


        public async Task<IList<UserProfile>> GetManagerHierarchy(Guid managerId)
        {
            List<UserProfile> lstManagers = new List<UserProfile>();
            var orgId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.OrganizationId);
            var roleId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.ManagerRoleId);
            var userStatus = ManagementDashboard_Utilites.Common.AppConstants.Status;
            List<Guid> userIds = new List<Guid>();
            //var applicationUsers = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Status == userStatus).ToList();

            //if (applicationUsers == null || applicationUsers.Count == 0)
            //{
            //    return new List<UserProfile>();
            //}

            //var userList = applicationUsers.Where(x => x.Roles.Contains(roleId)).ToList();

            //if (userList != null)
            //{
            var users = await base.Find(f => f.Organization == orgId && f.ManagerId == managerId);
            var topManager = await base.FindOne(x => x.Organization == orgId && x.User == managerId);

            if (topManager != null)
            {
                userIds.Add(topManager.User);

                foreach (var item in users)
                {
                    var allUsers = await base.Find(f => f.Organization == orgId);

                    var userIDs = await GetSubManagerIds(allUsers, item.User);
                    userIds.AddRange(userIDs);
                }

               userIds = userIds.Distinct().ToList();

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
            //}

            return lstManagers;
        }

        private async Task<List<Guid>> GetSubManagerIds(IList<UserProfile> users, Guid managerId)
        {
            List<Guid> userIds = new() { };

            foreach (var item in users)
            {
                if (item.ManagerId == managerId)
                {
                    userIds.Add(managerId);
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

            var users = await base.Find(f => f.Organization == new Guid(AppConstants.OrganizationId) && f.ManagerId == id);

            return (List<UserProfile>)users;
        }

        public async Task<UserProfile> GetEmployeeByName(string name)
        {
            Expression<Func<UserProfile, bool>> expression = u => u.FirstName == name;
            var result = await base.FindOne(expression);
            return result;
        }

    }
}

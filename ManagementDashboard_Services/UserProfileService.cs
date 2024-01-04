using ManagementDashboard_Entities;
using ManagementDashboard_Services;
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
        private readonly ProjectService _projectService;
        public UserProfileService(ApplicationSettings settings, UserManager<ApplicationUser> userManager, ProjectService projectService) : base(settings)
        {
            _userManager = userManager;
            _projectService = projectService;
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

            var managerRoleId = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Id == managerId && f.Status == userStatus).FirstOrDefault();

            if (managerRoleId == null)
            {
                return new List<UserProfile>();
            }

            userIds.Add(managerId);
            var allUsers = await base.Find(f => f.Organization == orgId && f.Status == userStatus);

            var userIDs = GetSubManagerIds(allUsers, managerId, orgId, ref userIds);
            userIds.AddRange(userIDs);

            userIds = userIds.Distinct().ToList();

            if (userIds != null)
            {
                lstManagers = (await Find(f => f.Organization == orgId
        && userIds.Contains(f.User)))
                .Select(s => new UserProfile()
                {
                    User = s.User,
                    UserCode = s.UserCode,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                }).ToList();
            }

            return lstManagers;
        }

        private List<Guid> GetSubManagerIds(IList<UserProfile> users, Guid managerId, Guid orgId, ref List<Guid> userIds)
        {
            var roleId = new Guid(ManagementDashboard_Utilites.Common.AppConstants.ManagerRoleId);
            var userStatus = ManagementDashboard_Utilites.Common.AppConstants.Status;

            foreach (var item in users)
            {
                if (item.ManagerId == managerId)
                {
                    userIds.Add(managerId);
                    var guid = item.User;
                    if (users.Where(x => x.ManagerId == guid).Any())
                    {
                        userIds.Add(item.User);
                        var sublist = users.Where(x => x.ManagerId == guid).ToList();
                        foreach (var subItem in sublist)
                        {
                            userIds = GetSubManagerIds(users, subItem.User, orgId, ref userIds);
                        }
                    }
                    else
                    {
                        var managerRoleId = _userManager.Users?.Where(f => f.OrganizationId == orgId && f.Id == guid && f.Status == userStatus).FirstOrDefault();
                        if (managerRoleId != null && managerRoleId.Roles.Contains(roleId))
                        {
                            userIds.Add(item.User);
                        }
                    }
                }
            }
            return userIds;
        }

        public async Task<List<UserProfile>> GetEmployeesListByManager(Guid id)
        {
            List<UserProfile> lstEmployees = new List<UserProfile>();
            List<UserProfile> lstManagers = new List<UserProfile>();

            lstManagers.AddRange(await GetManagerHierarchy(id));

            foreach (var manager in lstManagers)
            {
                var users = await base.Find(f => f.Organization == new Guid(AppConstants.OrganizationId) && f.ManagerId == manager.User);
                lstEmployees.AddRange(users);
            }
            lstEmployees = lstEmployees.Distinct().ToList();
            return lstEmployees;
        }

        public async Task<List<UserProfile>> GetEmployeesListByCustomer(Guid managerId, Guid customerId)
        {
            List<UserProfile> lstEmployees = new List<UserProfile>();
            //Get employees under the manager
            var emp = await GetEmployeesListByManager(managerId);

            if (emp != null)
            {
                //List of projects
                var projects = await _projectService.GetProjects(customerId);

                if (projects != null)
                {
                    //Get employees working in that project
                    var empProj = emp.Where(x => x.Projects.Any(userProject => projects.Any(externalProject => externalProject.Id == userProject))).ToList();
                }                              
            }
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

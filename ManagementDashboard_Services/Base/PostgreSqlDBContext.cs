﻿using ManagementDashboard_Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Services
{
    public class PostgreSQLDbContext : DbContext
    {
        public PostgreSQLDbContext(DbContextOptions<PostgreSQLDbContext> options)
            : base(options)
        {
        }

        public DbSet<Postgre_Customer> customer { get; set; }
        public DbSet<Postgre_Project> project { get; set; }
        public DbSet<Postgre_UserInfo> userinfo_mgmtapp { get; set; }

        public async Task<Postgre_Customer> GetCustomerByIdAsync(int projectId)
        {
            return await customer.FirstOrDefaultAsync(c => c.ProjectId == projectId);
        }

        public async Task<List<Postgre_Customer>> GetAllCustomersAsync()
        {
            return await customer.ToListAsync();
        }

        public async Task<Postgre_Customer> InsertCustomersAsync(Postgre_Customer customers)
        {
            await customer.AddAsync(customers);
            await SaveChangesAsync();
            return customers;
        }

        public async Task<Postgre_Project> InsertProjectAsync(Postgre_Project projects)
        {
            await project.AddAsync(projects);
            await SaveChangesAsync();
            return projects;
        }

        public async Task<Postgre_UserInfo> InsertUserAsync(Postgre_UserInfo user)
        {
            await userinfo_mgmtapp.AddAsync(user);
            await SaveChangesAsync();
            return user;
        }
    }
}
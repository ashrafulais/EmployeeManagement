using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        //Initial Seed Data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //map keys to the .net core identity
            //because we are overriding the method, we need to specify it in the base method
            base.OnModelCreating(modelBuilder);
            
            //use this code to enforce not to delete a record that contains foreign key connection (User , Roles , UserRoles : tables)

            foreach(var foreignKey in modelBuilder.Model.GetEntityTypes()
                                        .SelectMany(e => e.GetForeignKeys() ))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.LoadSeedData();
        }

    }
}

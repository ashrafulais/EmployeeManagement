using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void LoadSeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData
            (
                new Employee()
                {
                    Id = 1,
                    Name = "Apple",
                    Department = DepartmentEnum.IT,
                    Email = "mail@mail.com"
                },
                new Employee()
                {
                    Id = 2,
                    Name = "Orange",
                    Department = DepartmentEnum.HR,
                    Email = "mail@mail.com"
                }
            );
        }
    }
}

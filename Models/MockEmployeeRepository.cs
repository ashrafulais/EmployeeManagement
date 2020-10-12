using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> employees = new List<Employee>();

        public MockEmployeeRepository()
        {
            employees = new List<Employee>()
            {
                new Employee()
                {
                    Id=1, Name="Sam", Department="CSE", Email="mail@mail.com"
                },
                new Employee()
                {
                    Id=2, Name="Mun", Department="MED", Email="mail@mail.com"
                }
            };
        }

        public Employee GetEmployee(int Id)
        {
            return employees.FirstOrDefault(x => x.Id == Id);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return employees.AsEnumerable();
        }
    }
}

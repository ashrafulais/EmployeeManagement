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
                    Id=1, Name="Apple", Department=DepartmentEnum.IT, Email="mail@mail.com"
                },
                new Employee()
                {
                    Id=2, Name="Orange", Department=DepartmentEnum.HR, Email="mail@mail.com"
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

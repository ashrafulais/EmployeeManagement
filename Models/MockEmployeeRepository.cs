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

        public Employee Add(Employee employee)
        {
            employee.Id = employees.Max(e => e.Id) + 1;
            employees.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = employees.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                employees.Remove(employee);
            }
            return employee;
        }

        public Employee GetEmployee(int Id)
        {
            return employees.FirstOrDefault(x => x.Id == Id);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return employees.AsEnumerable();
        }

        public Employee Update(Employee employeeChanges)
        {
            Employee employee = employees.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }
    }
}

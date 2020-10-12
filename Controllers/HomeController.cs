using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository _repository { get; }

        public HomeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index()
        {
            return View();

            //return _repository.GetEmployee(1).Name;
        }
        public ViewResult Details()
        {
            Employee employeeModel = new Employee();
            employeeModel = _repository.GetEmployee(1);

            ViewBag.PageTitle = "Employee model";
            return View(employeeModel);
        }

    }
}

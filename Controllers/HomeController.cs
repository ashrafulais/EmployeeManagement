using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository repository { get; }

        public HomeController(IEmployeeRepository repository)
        {
            this.repository = repository;
        }


        public ViewResult Index()
        {
            var employeeList = repository.GetEmployees();
            return View(employeeList);

            //return repository.GetEmployee(1).Name;
        }

        public ViewResult Details(int? id)
        {
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = repository.GetEmployee(id??1),
                PageTitle = "Employee model"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if(ModelState.IsValid)
            {
                Employee newEmployee = repository.Add(employee);
                return RedirectToAction("details", new { id = newEmployee.Id});
            }
            return View();
        }

    }
}

﻿using EmployeeManagement.Models;
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
        private IEmployeeRepository _repository { get; }

        public HomeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }


        public ViewResult Index()
        {
            var employeeList = _repository.GetEmployees();
            return View(employeeList);

            //return _repository.GetEmployee(1).Name;
        }

        public ViewResult Details(int? id)
        {
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _repository.GetEmployee(id??1),
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
        public RedirectToActionResult Create(Employee employee)
        {
            Employee newEmployee = _repository.Add(employee);
            return RedirectToAction("details", new { id = newEmployee.Id});
        }

    }
}

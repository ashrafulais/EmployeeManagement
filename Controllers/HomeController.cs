using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Route("Home")]
    [Route("[controller]/[action]")] //token
    public class HomeController : Controller
    {
        private IEmployeeRepository _repository { get; }

        public HomeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }


        //[Route("Home")]
        //[Route("Index")]
        //[Route("[action]")]
        [Route("")]
        [Route("~/")]
        [Route("~/Home")]
        public ViewResult Index()
        {
            var employeeList = _repository.GetEmployees();
            return View(employeeList);

            //return _repository.GetEmployee(1).Name;
        }

        //[Route("Details/{id?}")]
        //[Route("[action]/{id?}")]
        [Route("{id?}")]

        public ViewResult Details(int? id)
        {
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _repository.GetEmployee(id??1),
                PageTitle = "Employee model"
            };

            return View(homeDetailsViewModel);
        }

    }
}

using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private IEmployeeRepository repository { get; }

        public HomeController(IEmployeeRepository repository,
            IWebHostEnvironment hostingEnvironment )
        {
            this.repository = repository;
            this.hostingEnvironment = hostingEnvironment;
        }


        public ViewResult Index()
        {
            var employeeList = repository.GetEmployees();
            return View(employeeList);

            //return repository.GetEmployee(1).Name;
        }

        public ViewResult Details(int? id)
        {
            Employee employee = repository.GetEmployee(id ?? -1);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
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
        public IActionResult Create(EmployeeCreateViewModel employeeModel)
        {
            if(ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedImage(employeeModel);


                Employee newEmployee = new Employee
                {
                    Name = employeeModel.Name,
                    Email = employeeModel.Email,
                    Department = employeeModel.Department,
                    PhotoPath = uniqueFileName
                };

                repository.Add(newEmployee);

                return RedirectToAction("details", new { id = newEmployee.Id});
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee employee = repository.GetEmployee(id);

            //return Json(employee);

            if (employee != null)
            {
                EmployeeEditViewModel editModel = new EmployeeEditViewModel
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Department = employee.Department,
                    ExistingPhotoPath = employee.PhotoPath
                };

                return View(editModel);
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                Employee employee = repository.GetEmployee(employeeModel.Id);
                if(employee is null)
                {
                    return RedirectToAction("index");
                }

                employee.Name = employeeModel.Name;
                employee.Email = employeeModel.Email;
                employee.Department = employeeModel.Department;
                
                if(employeeModel.Photo != null)
                {
                    if(employeeModel.ExistingPhotoPath != null)
                    {
                        string existingFilePath = Path.Combine(hostingEnvironment.WebRootPath, "img", employeeModel.ExistingPhotoPath);

                        System.IO.File.Delete(existingFilePath);
                    }
                    employee.PhotoPath = ProcessUploadedImage(employeeModel);
                }

                repository.Update(employee);

                return RedirectToAction("details", new { id = employee.Id });
            }
            return View();
        }

        private string ProcessUploadedImage(EmployeeCreateViewModel employeeModel)
        {
            string uniqueFileName = null;
            if (employeeModel.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "img"); //www folder
                uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeModel.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using(var fileStream = new FileStream(filePath, FileMode.Create) )
                {
                    employeeModel.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}

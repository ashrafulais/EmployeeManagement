using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required,
         MaxLength(80, ErrorMessage = "Name cannot exceed 80 characters"),
         MinLength(2, ErrorMessage = "Name must contain more than 1 character")]
        public string Name { get; set; }

        [Required,
         RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
            ErrorMessage = "Invalid email format")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        [Required]
        public DepartmentEnum? Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}

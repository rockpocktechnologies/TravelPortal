using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TravelPortal.Classes;

namespace TravelPortal.Models
{
    public class EmployeeDepartmentViewModel
    {

        [Required]
        [Display(Name = "Select Department")]
        public int DepartmentID { get; set; }

        [Required]
        [Display(Name = "Select Employee")]
        public int EmployeeID { get; set; }

        public List<Department> Departments { get; set; }
        public List<Employee> Employees { get; set; }

        public EmployeeDepartmentViewModel()
        {
            Departments = new List<Department>();
            Employees = new List<Employee>();
        }

       
    }

}




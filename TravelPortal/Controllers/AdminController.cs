using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TravelPortal.Models;
using TravelPortal.Classes;

namespace TravelPortal.Controllers
{

    public class AdminController : Controller
    {
        private string _connectionString; // Your database connection string

        public AdminController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        [HttpGet]
        public async Task<IActionResult> AssociateEmployeeDepartment()
        {
            EmployeeDepartmentViewModel model = new EmployeeDepartmentViewModel();

            // Load departments and employees from the database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Fetch departments
                using (SqlCommand departmentCmd = new SqlCommand("SELECT DepartmentID, DepartmentName FROM trvl_tblDepartments", connection))
                {
                    using (SqlDataReader departmentReader = departmentCmd.ExecuteReader())
                    {
                        while (departmentReader.Read())
                        {
                            model.Departments.Add(new Department
                            {
                                DepartmentID = departmentReader.GetInt32(0),
                                DepartmentName = departmentReader.GetString(1)
                            });
                        }
                    }
                }

                // Fetch employees
                using (SqlCommand employeeCmd = new SqlCommand("SELECT EmployeeID, EmployeeName FROM EmployeeDetails", connection))
                {
                    using (SqlDataReader employeeReader = employeeCmd.ExecuteReader())
                    {
                        while (employeeReader.Read())
                        {
                            model.Employees.Add(new Employee
                            {
                                EmployeeID = employeeReader.GetInt32(0),
                                EmployeeName = employeeReader.GetString(1)
                            });
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssociateEmployeeDepartment(EmployeeDepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the association in the trvl_tblEmployeeDepartments table
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO trvl_tblEmployeeDepartments (EmployeeID, DepartmentID, IsAdmin) VALUES (@EmployeeID, @DepartmentID, 0)", connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                        cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("AssociateEmployeeDepartment");
            }

            return View(model);
        }
    }

}

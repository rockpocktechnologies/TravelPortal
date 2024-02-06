using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using TravelPortal.Models;
using Microsoft.Kiota.Abstractions;
using NuGet.Protocol.Plugins;
using System.Diagnostics.Metrics;

namespace TravelPortal.Controllers
{
  
    public class ReportController : Controller
    {
        private readonly IConfiguration _configuration;
        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            //try
           // {
                // Fetch master types from the database using ADO.NET and stored procedure
                List<ReportType> reportTypes = GetReportTypesFromDatabase();

                // Pass the master types to the view
                ViewBag.ReportTypes = reportTypes;

                return View();
            //}
            //catch (Exception ex)
            //{
            //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            //    string exceptionMessage = $"Exception in method '{methodName}'";

            //    // Log or rethrow the exception with the updated message
            //    //var errorLogger = new CustomErrorLog(_configuration);
            //    //errorLogger.LogError(ex, exceptionMessage);
            //    return Json(new { error = ex.Message });
            //}
        }
        private List<ReportType> GetReportTypesFromDatabase()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            List<ReportType> ReportTypes = new List<ReportType>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_SPGetReports", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReportType report = new ReportType
                            {
                                Id = (int)reader["Id"],
                                ReportName = reader["ReportName"].ToString(),
                                // Add other properties as needed
                            };

                            ReportTypes.Add(report);
                        }
                    }
                }
            }

            return ReportTypes;
        }

    }
}

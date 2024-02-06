using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TravelPortal.Models;

namespace TravelPortal.Controllers
{
    public class EmailLogController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmailLogController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(int? topCount)
        {
            // Default to 200 logs if topCount is not provided
            int numberOfLogs = topCount ?? 200;

            List<EmailLogModel> emailLogs = GetEmailLogs(numberOfLogs);

            return View(emailLogs);
        }

        private List<EmailLogModel> GetEmailLogs(int topCount)
        {
            List<EmailLogModel> emailLogs = new List<EmailLogModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_sp_GetEmailLogs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TopCount", topCount);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map data from SqlDataReader to your EmailLogModel
                            // Replace EmailLogModel with your actual model class
                            EmailLogModel log = new EmailLogModel
                            {
                                EmailLogId = reader.GetInt32(reader.GetOrdinal("EmailLogId")),
                                ToEmail = reader.GetString(reader.GetOrdinal("ToEmail")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Body = reader.GetString(reader.GetOrdinal("Body")),
                                SentDate = reader.GetDateTime(reader.GetOrdinal("SentDate"))
                                // Add more properties as needed
                            };

                            emailLogs.Add(log);
                        }
                    }
                }
            }

            return emailLogs;
        }


        [HttpGet]
        public IActionResult GetEmailBody(int emailLogId)
        {
            // Fetch HTML content for the specified email log ID
            string htmlContent = GetEmailBodyFromDatabase(emailLogId);

            return Content(htmlContent, "text/html");
        }
        private string GetEmailBodyFromDatabase(int emailLogId)
        {
            string htmlContent = string.Empty;

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_sp_GetEmailBody", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmailLogId", emailLogId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            htmlContent = reader.GetString(reader.GetOrdinal("Body"));
                        }
                    }
                }
            }

            return htmlContent;
        }

    }
}

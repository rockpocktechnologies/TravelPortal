
using System;
using System.Data;
using System.Data.SqlClient;

namespace TravelPortal.Classes
{
    public class ErrorLogger
    {
        private readonly string connectionString;

        public ErrorLogger(IConfiguration _configuration)
        {
            // Retrieve the connection string from appsettings.json
            connectionString = _configuration.GetConnectionString("ConnectionString");
        }

        public void LogError(Exception ex, string additionalInfo = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_SPInsertErrorLog", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                    command.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? string.Empty);
                    command.Parameters.AddWithValue("@AdditionalInfo", additionalInfo ?? string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
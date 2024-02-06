using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TravelPortal.Classes
{
    
    public class TokenService
    {
        private IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateNewTravelRequestToken(int newTravelRequestId,
            int requestType)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            string uniqueToken = string.Empty;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("trvl_SPGenerateTravelRequestToken", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TravelRequestId", newTravelRequestId);
                    command.Parameters.AddWithValue("@RequestType", requestType);
                    
                    var tokenParameter = new SqlParameter("@Token", SqlDbType.NVarChar, 255)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(tokenParameter);

                    command.ExecuteNonQuery();

                    uniqueToken = tokenParameter.Value.ToString();
                }
            }

            return uniqueToken;
        }
    }


}

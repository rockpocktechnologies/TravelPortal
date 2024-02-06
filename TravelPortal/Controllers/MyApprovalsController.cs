using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TravelPortal.Models;
using TravelPortal.Classes;
using Microsoft.AspNetCore.Authorization;

//[Authorize]
public class MyApprovalsController : Controller
{
    private readonly IConfiguration _configuration;

    public MyApprovalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        List<MyApprovalModel> travelRequestDetails = new List<MyApprovalModel>();

        using (SqlConnection connection =
              new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("trvl_SPGetMyApprovalDetails", connection))
            {
                string SEmployeeNo = HttpContext.Session.GetString("SEmployeeNo");
                bool IsEmployeeTravelDesk = CheckLoggedInPersonIsTravelDesk(SEmployeeNo);
                bool isLoggedEmployeeAccountant = false;
                bool isLoggedEmployeeAdmin = false;

                if(HttpContext.Session.GetString("isForVolTravelEmail")  == "true")
                {
                    IsEmployeeTravelDesk = true;
                }

                if (!IsEmployeeTravelDesk)
                {
                    isLoggedEmployeeAdmin = CheckLoggedInPersonIsAdmin(SEmployeeNo);
                    if (!isLoggedEmployeeAdmin)
                    {
                        isLoggedEmployeeAccountant = CheckLoggedInPersonIsAccontant(SEmployeeNo);

                    }
                }
                //if(SEmployeeNo == "111114") //temp{
                //{
                //    IsEmployeeTravelDesk = true;
                //}
                if (IsEmployeeTravelDesk)
                {
                    HttpContext.Session.SetString("SIsTravelDesk", "true");
                }
                else if (isLoggedEmployeeAccountant)
                {
                    HttpContext.Session.SetString("SIsAccountant", "true");
                }
                else if (isLoggedEmployeeAdmin)
                {
                    HttpContext.Session.SetString("SIsAdmin", "true");
                }


                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmployeeNo", SEmployeeNo);
                command.Parameters.AddWithValue("@IsEmployeeTravelDesk", IsEmployeeTravelDesk);
                command.Parameters.AddWithValue("@IsLoggedEmployeeAccountant", isLoggedEmployeeAccountant);
                command.Parameters.AddWithValue("@IsLoggedEmployeeAdmin", isLoggedEmployeeAdmin);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        travelRequestDetails.Add(new MyApprovalModel
                        {
                            RequestId = reader.IsDBNull(reader.GetOrdinal("RequestId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RequestId")),
                            RequestToken = reader.IsDBNull(reader.GetOrdinal("RequestToken")) ? string.Empty : reader.GetString(reader.GetOrdinal("RequestToken")),
                            TravelRequestNumber = reader.IsDBNull(reader.GetOrdinal("TravelRequestNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("TravelRequestNumber")),
                            FromPlace = reader.IsDBNull(reader.GetOrdinal("FromPlace")) ? string.Empty : reader.GetString(reader.GetOrdinal("FromPlace")),
                            ToPlace = reader.IsDBNull(reader.GetOrdinal("ToPlace")) ? string.Empty : reader.GetString(reader.GetOrdinal("ToPlace")),
                            DateOfJourney = reader.IsDBNull(reader.GetOrdinal("DateOfJourney")) ? DateTime.MinValue.ToString("dd-MM-yyyy") : reader.GetDateTime(reader.GetOrdinal("DateOfJourney")).ToString("dd-MM-yyyy"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString(reader.GetOrdinal("Status")),
                            EmployeeNo = reader.IsDBNull(reader.GetOrdinal("EmployeeNo")) ? string.Empty : reader.GetString(reader.GetOrdinal("EmployeeNo")),
                            EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? string.Empty : reader.GetString(reader.GetOrdinal("EmployeeName")),
                            Mode = reader.IsDBNull(reader.GetOrdinal("Mode")) ? string.Empty : reader.GetString(reader.GetOrdinal("Mode")),
                            TravelDestination = reader.IsDBNull(reader.GetOrdinal("TravelDestination")) ? string.Empty : reader.GetString(reader.GetOrdinal("TravelDestination")),
                            Purpose = reader.IsDBNull(reader.GetOrdinal("Purpose")) ? string.Empty : reader.GetString(reader.GetOrdinal("Purpose")),
                            RequestedFor = reader.IsDBNull(reader.GetOrdinal("RequestedFor")) ? string.Empty : reader.GetString(reader.GetOrdinal("RequestedFor")),
                            Ticket_Booked_By = reader.IsDBNull(reader.GetOrdinal("Ticket_Booked_By")) ? string.Empty : reader.GetString(reader.GetOrdinal("Ticket_Booked_By")),
                            TravelType = reader.IsDBNull(reader.GetOrdinal("TravelType")) ? string.Empty : reader.GetString(reader.GetOrdinal("TravelType")),
                            Project = reader.IsDBNull(reader.GetOrdinal("Project")) ? string.Empty : reader.GetString(reader.GetOrdinal("Project")),
                            ApprovalToken = reader.IsDBNull(reader.GetOrdinal("ApprovalToken")) ? string.Empty : reader.GetString(reader.GetOrdinal("ApprovalToken"))


                        });
                    }
                }
            }
        }

        return View(travelRequestDetails);
    }

    public bool CheckLoggedInPersonIsTravelDesk(string employeeNumber)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("trvl_SPCheckTravelDeskPerson", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add the @EmployeeNumber parameter
                cmd.Parameters.Add(new SqlParameter("@EmployeeNumber", SqlDbType.VarChar, 50));
                cmd.Parameters["@EmployeeNumber"].Value = employeeNumber;

                // Add the output parameter for the result
                SqlParameter outputParameter = new SqlParameter("@IsTravelDeskPerson", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParameter);

                cmd.ExecuteNonQuery();

                bool isTravelDeskPerson = Convert.ToBoolean(cmd.Parameters["@IsTravelDeskPerson"].Value);

                return isTravelDeskPerson;
            }
        }
    }


     public bool CheckLoggedInPersonIsAccontant(string employeeNumber)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("trvl_SPCheckAccountsPerson", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add the @EmployeeNumber parameter
                cmd.Parameters.Add(new SqlParameter("@EmployeeNumber", SqlDbType.VarChar, 50));
                cmd.Parameters["@EmployeeNumber"].Value = employeeNumber;

                // Add the output parameter for the result
                SqlParameter outputParameter = new SqlParameter("@IsAccountPerson", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParameter);

                cmd.ExecuteNonQuery();

                bool isAccountsPerson = Convert.ToBoolean(cmd.Parameters["@IsAccountPerson"].Value);

                return isAccountsPerson;
            }
        }
    }


    
         public bool CheckLoggedInPersonIsAdmin(string employeeNumber)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("trvl_SPCheckAdminPerson", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add the @EmployeeNumber parameter
                cmd.Parameters.Add(new SqlParameter("@EmployeeNumber", SqlDbType.VarChar, 50));
                cmd.Parameters["@EmployeeNumber"].Value = employeeNumber;

                // Add the output parameter for the result
                SqlParameter outputParameter = new SqlParameter("@IsAdminPerson", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParameter);

                cmd.ExecuteNonQuery();

                bool IsAdminPerson = Convert.ToBoolean(cmd.Parameters["@IsAdminPerson"].Value);

                return IsAdminPerson;
            }
        }
    }
}

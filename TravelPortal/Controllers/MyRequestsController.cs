using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TravelPortal.Models;
using TravelPortal.Classes;
using Microsoft.Graph.Beta.Models;
using static TravelPortal.Controllers.RequestApprovalController;
using TravelPortal.Controllers;

public class MyRequestsController : Controller
{
    private readonly IConfiguration _configuration;

    public MyRequestsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        List<MyRequestsModel> travelRequestDetails = new List<MyRequestsModel>();

        using (SqlConnection connection =
              new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("trvl_SPGetMyRequestDetails", connection))
            {
              string SEmployeeNo =  HttpContext.Session.GetString("SEmployeeNo");

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmployeeNo", SEmployeeNo);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        travelRequestDetails.Add(new MyRequestsModel
                        {
                            RequestId = reader.IsDBNull(reader.GetOrdinal("RequestId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RequestId")),
                            RequestToken = reader.IsDBNull(reader.GetOrdinal("RequestToken")) ? string.Empty : reader.GetString(reader.GetOrdinal("RequestToken")),
                            ApprovalToken = reader.IsDBNull(reader.GetOrdinal("ApprovalToken")) ? string.Empty : reader.GetString(reader.GetOrdinal("ApprovalToken")),
                            TravelRequestNumber = reader.IsDBNull(reader.GetOrdinal("TravelRequestNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("TravelRequestNumber")),
                            FromPlace = reader.IsDBNull(reader.GetOrdinal("FromPlace")) ? string.Empty : reader.GetString(reader.GetOrdinal("FromPlace")),
                            ToPlace = reader.IsDBNull(reader.GetOrdinal("ToPlace")) ? string.Empty : reader.GetString(reader.GetOrdinal("ToPlace")),
                            DateOfJourney = reader.IsDBNull(reader.GetOrdinal("DateOfJourney")) ? DateTime.MinValue.ToString("dd-MM-yyyy") : reader.GetDateTime(reader.GetOrdinal("DateOfJourney")).ToString("dd-MM-yyyy"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString(reader.GetOrdinal("Status")),
                            //Emp_No = reader.IsDBNull(reader.GetOrdinal("Emp_No")) ? string.Empty : reader.GetString(reader.GetOrdinal("Emp_No")),
                            //Emp_Name = reader.IsDBNull(reader.GetOrdinal("Emp_Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Emp_Name")),
                            Mode = reader.IsDBNull(reader.GetOrdinal("Mode")) ? string.Empty : reader.GetString(reader.GetOrdinal("Mode")),
                            TravelDestination = reader.IsDBNull(reader.GetOrdinal("TravelDestination")) ? string.Empty : reader.GetString(reader.GetOrdinal("TravelDestination")),
                            Purpose = reader.IsDBNull(reader.GetOrdinal("Purpose")) ? string.Empty : reader.GetString(reader.GetOrdinal("Purpose")),
                            RequestedFor = reader.IsDBNull(reader.GetOrdinal("RequestedFor")) ? string.Empty : reader.GetString(reader.GetOrdinal("RequestedFor")),
                            Ticket_Booked_By = reader.IsDBNull(reader.GetOrdinal("Ticket_Booked_By")) ? string.Empty : reader.GetString(reader.GetOrdinal("Ticket_Booked_By")),
                            TravelType = reader.IsDBNull(reader.GetOrdinal("TravelType")) ? string.Empty : reader.GetString(reader.GetOrdinal("TravelType")),
                            Project = reader.IsDBNull(reader.GetOrdinal("Project")) ? string.Empty : reader.GetString(reader.GetOrdinal("Project")),
                            IsSubmitted = reader.IsDBNull(reader.GetOrdinal("IsSubmitted")) ? false : reader.GetBoolean(reader.GetOrdinal("IsSubmitted"))

                        });
                    }
                }
            }
        }

        return View(travelRequestDetails);
    }

    [HttpPost]
    public JsonResult CancelRequest(string token)
    {
        bool success = CancelTravelRequestInDatabase(token); // Call your ADO.NET method here

        return Json(new { success });
    }

    private bool CancelTravelRequestInDatabase(string token)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("trvl_SPCancelMyRequest", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add parameters for the stored procedure
                cmd.Parameters.Add(new SqlParameter("@Token", SqlDbType.VarChar, 50)).Value = token;
                cmd.Parameters.Add(new SqlParameter("@IsSentMailToTravelDeskForCancelation", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@IsSentMailToAdminForCancelation", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@TravelRequestId", SqlDbType.Int) { Direction = ParameterDirection.Output });
                

                // Execute the stored procedure
                cmd.ExecuteNonQuery();

                var IsSentMailToTravelDeskForCancelation = (bool)cmd.Parameters["@IsSentMailToTravelDeskForCancelation"].Value;
                var IsSentMailToAdminForCancelation = (bool)cmd.Parameters["@IsSentMailToAdminForCancelation"].Value;
                var TravelRequestId = (int)cmd.Parameters["@TravelRequestId"].Value;


                if (IsSentMailToTravelDeskForCancelation)
                {

                    List<TravelDeskPersonnel> travelDeskPersonnelList = GetTravelDeskDepartmentPersonnel();

                    if (travelDeskPersonnelList.Count > 0)
                    {

                       

                        foreach (var personnel in travelDeskPersonnelList)
                        {
                            Console.WriteLine($"Employee ID: {personnel.EmployeeID}");
                            Console.WriteLine($"Employee Name: {personnel.EmployeeName}");
                            Console.WriteLine($"Employee Email: {personnel.EmployeeEmail}");
                            Console.WriteLine();

                            SendEmailController eml = new SendEmailController(_configuration);
                            eml.SendEmailToTravelDeskForCancelTicket(TravelRequestId, personnel.EmployeeEmail,
                                personnel.EmployeeName);
                        }



                    }
                }
                else if (IsSentMailToAdminForCancelation)
                {
                    
                        List<AdminPersonnel> adminList = GetAdminDepartmentPersonnel();

                        if (adminList.Count > 0)
                        {
                            foreach (var personnel in adminList)
                            {
                                SendEmailController eml = new SendEmailController(_configuration);
                                eml.SendEmailToTravelDeskForCancelTicket(TravelRequestId, personnel.EmployeeEmail,
                                    personnel.EmployeeName);

                            }
                        }

                }

                return true; // Assuming success
            }
        }

    }
    public List<AdminPersonnel> GetAdminDepartmentPersonnel()
    {
        List<AdminPersonnel> personnelList = new List<AdminPersonnel>();

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("trvl_SPGetGetAdminDepartmentPersonnel", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AdminPersonnel personnel = new AdminPersonnel
                        {
                            EmployeeID = reader.GetString(reader.GetOrdinal("EmployeeID")),
                            EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                            EmployeeEmail = reader.GetString(reader.GetOrdinal("EmployeeEmail"))
                        };

                        personnelList.Add(personnel);
                    }
                }
            }
        }

        return personnelList;
    }

    public List<TravelDeskPersonnel> GetTravelDeskDepartmentPersonnel()
    {
        List<TravelDeskPersonnel> personnelList = new List<TravelDeskPersonnel>();

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("trvl_SPGetTravelDeskDepartmentEmployees", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TravelDeskPersonnel personnel = new TravelDeskPersonnel
                        {
                            EmployeeID = reader.GetString(reader.GetOrdinal("EmployeeID")),
                            EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                            EmployeeEmail = reader.GetString(reader.GetOrdinal("EmployeeEmail"))
                        };

                        personnelList.Add(personnel);
                    }
                }
            }
        }

        return personnelList;
    }

}

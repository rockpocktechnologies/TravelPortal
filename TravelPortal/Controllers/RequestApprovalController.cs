using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using TravelPortal.Classes;

namespace TravelPortal.Controllers
{
    public class RequestApprovalController : Controller
    {
        private readonly IConfiguration _configuration; // Inject your configuration if needed

        public RequestApprovalController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("RequestApproval/ApproveRequest")]
        public IActionResult ApproveRequest(string token)
        {
            string requestNumber = "";
            int statusID = 0;
            if (token == null)
            {
                return View("RequestNotFound");
            }

            try
            {
               
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();
                    int RequestType = 1;
                   

                    if(token == null)
                    {
                        return View("RequestNotFound");
                    }

                    if (token.Contains("Expense-"))//Expense token
                    {
                        RequestType = 2;
                    }
                   

                        // Create a command to call the stored procedure
                        using (SqlCommand command = new SqlCommand("trvl_SPAproveTravelRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Token", token);
                        command.Parameters.AddWithValue("@RequestType", RequestType);
                        command.Parameters.Add(new SqlParameter("@NextEmailID", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });

                        // Add output parameter for status ID
                        command.Parameters.Add(new SqlParameter("@Status_ID", SqlDbType.Int) { Direction = ParameterDirection.Output });

                        // Add output parameter for IsNotFound
                        command.Parameters.Add(new SqlParameter("@IsNotFound", SqlDbType.Bit) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@NameOfReceiver", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@TravelRequestId", SqlDbType.Int) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@IsSentMailToTravel", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                       
                        command.Parameters.Add(new SqlParameter("@IsAdvanceAmountMailToAccounts", SqlDbType.Bit) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@IsSentMailToAdmin", SqlDbType.Bit) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@IsSentMailToInsurance", SqlDbType.Bit) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@RequestOwnerName", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@RequestOwnerEmail", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });
                        
                        command.Parameters.Add(new SqlParameter("@RequestNumber", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });

                        command.Parameters.Add(new SqlParameter("@IsMakeStatusBookedWhenbySelf", SqlDbType.Bit) { Direction = ParameterDirection.Output });


                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Check if the request was not found
                        bool isNotFound = (bool)command.Parameters["@IsNotFound"].Value;

                        if (isNotFound)
                        {
                            return View("RequestNotFound");
                        }
                        else
                        {
                            var nextEmailID = command.Parameters["@NextEmailID"].Value.ToString();
                             statusID = (int)command.Parameters["@Status_ID"].Value;
                            var NameOfReceiver = command.Parameters["@NameOfReceiver"].Value.ToString();
                            var TravelRequestId = (int)command.Parameters["@TravelRequestId"].Value;
                            var IsSentMailToTravelDesk = (bool)command.Parameters["@IsSentMailToTravel"].Value;
                            var IsAdvanceAmountMailToAccounts = (bool)command.Parameters["@IsAdvanceAmountMailToAccounts"].Value;
                            var IsSentMailToAdmin = (bool)command.Parameters["@IsSentMailToAdmin"].Value;
                            var IsSentMailToInsurance = (bool)command.Parameters["@IsSentMailToInsurance"].Value;
                            var RequestOwnerName = command.Parameters["@RequestOwnerName"].Value.ToString();
                            var RequestOwnerEmail = command.Parameters["@RequestOwnerEmail"].Value.ToString();
                            requestNumber = command.Parameters["@RequestNumber"].Value.ToString();
                            var IsMakeStatusBookedWhenbySelf = (bool)command.Parameters["@IsMakeStatusBookedWhenbySelf"].Value;

                            LogApprovalData(TravelRequestId, statusID, NameOfReceiver,
                                IsSentMailToTravelDesk, IsAdvanceAmountMailToAccounts,
                                IsSentMailToAdmin, IsSentMailToInsurance, RequestOwnerName,
                                "", requestNumber, IsMakeStatusBookedWhenbySelf,token);


                            if (RequestType == 2)//Expense token
                            {

                                if(statusID == 22 )// Expense Approved by Accountant
                                {
                                    SendEmailController eml = new SendEmailController(_configuration);
                                    eml.SendEmailToUserThatExpenseApproved(TravelRequestId, RequestOwnerName,
                                        RequestOwnerEmail,"Accounts Team");
                                }
                                else
                                {
                                    TokenService tkn = new TokenService(_configuration);
                                    string tokenGuid = tkn.CreateNewTravelRequestToken(TravelRequestId, 2);
                                    var approvalUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/ApproveRequest"}";
                                    var rejectUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/RejectRequest"}";


                                    List<AccountsPersonnel> accountsPersonnelList = GetAccountsDepartmentPersonnel();

                                    if (accountsPersonnelList.Count > 0)
                                    {

                                        foreach (var personnel in accountsPersonnelList)
                                        {


                                            SendEmailController eml = new SendEmailController(_configuration);
                                            eml.SendEmailToAccounForExpenseApproval(TravelRequestId, personnel.EmployeeEmail,
                                                personnel.EmployeeName,"", approvalUrl,rejectUrl, tokenGuid);

                                        }
                                    }

                                    SendEmailController emlnew = new SendEmailController(_configuration);
                                    emlnew.SendEmailToUserThatExpenseApproved(TravelRequestId, RequestOwnerName,
                                        RequestOwnerEmail, "Manager");
                                }
                                
                            }
                            else { 
                            // Retrieve the output values
                            

                            TokenService tkn = new TokenService(_configuration);
                            string tokenGuid = tkn.CreateNewTravelRequestToken(TravelRequestId, 1);
                            var approvalUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/ApproveRequest"}";
                            var rejectUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/RejectRequest"}";


                            if (statusID == 6)//approved
                            {
                                    if (IsSentMailToTravelDesk)
                                    {
                                        UpdatePendingTravelRequest(TravelRequestId);
                                    }

                                    if (IsSentMailToAdmin)
                                    {
                                        SPUpdateStatusPendingWithAdmin(TravelRequestId);
                                    }

                                    if (IsMakeStatusBookedWhenbySelf)
                                    {
                                        SPUpdateStatusTicketIsBooked(TravelRequestId);
                                    }


                                    if (IsAdvanceAmountMailToAccounts)
                                {
                                    List<AccountsPersonnel> accountsPersonnelList = GetAccountsDepartmentPersonnel();

                                    if (accountsPersonnelList.Count > 0)
                                    {

                                        foreach (var personnel in accountsPersonnelList)
                                        {


                                            SendEmailController eml = new SendEmailController(_configuration);
                                            eml.SendEmailTAccounforAdvanceAmount(TravelRequestId, personnel.EmployeeEmail,
                                                personnel.EmployeeName);

                                        }
                                    }
                                }

                                if (IsSentMailToTravelDesk)
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
                                            eml.SendEmailToTravelDeskForBooking(TravelRequestId, personnel.EmployeeEmail,
                                                personnel.EmployeeName);
                                        }

                                    

                                    }
                                }

                                if (IsSentMailToAdmin)
                                {

                                    List<AdminPersonnel> adminList = GetAdminDepartmentPersonnel();

                                    if (adminList.Count > 0)
                                    {
                                        foreach (var personnel in adminList)
                                        {
                                            SendEmailController eml = new SendEmailController(_configuration);
                                            eml.SendEmailToAdmin(TravelRequestId, personnel.EmployeeEmail,
                                                personnel.EmployeeName);

                                        }
                                    }
                                }

                                if (IsSentMailToInsurance)
                                {
                                    List<InsurancePersonnel> insurancelList = GetInsuranceDepartmentPersonnel();

                                    if (insurancelList.Count > 0)
                                    {
                                        foreach (var personnel in insurancelList)
                                        {

                                            SendEmailController eml = new SendEmailController(_configuration);
                                            eml.SendEmailToInsurance(TravelRequestId, personnel.EmployeeEmail,
                                                personnel.EmployeeName);
                                        }


                                    }
                                }


                            }
                            else if (statusID == 5)//Directors Approval
                            {
                                List<DirectorPersonnel> directorPersonnelList = GetDirectorDepartmentPersonnel();

                                if (directorPersonnelList.Count > 0)
                                {
                                    foreach (var personnel in directorPersonnelList)
                                    {
                                        SendEmailController eml = new SendEmailController(_configuration);
                                        eml.SendApprovalEmail(TravelRequestId, personnel.EmployeeName,
                                            personnel.EmployeeEmail,
                                            tokenGuid, approvalUrl, rejectUrl);

                                        //Console.WriteLine($"Employee ID: {personnel.EmployeeID}");
                                        //Console.WriteLine($"Employee Name: {personnel.EmployeeName}");
                                        //Console.WriteLine($"Employee Email: {personnel.EmployeeEmail}");
                                        //Console.WriteLine();
                                    }
                                }
                            }
                            else if (nextEmailID != null && nextEmailID != "")
                            {

                                SendEmailController eml = new SendEmailController(_configuration);
                                eml.SendApprovalEmail(TravelRequestId, NameOfReceiver, nextEmailID,
                                    tokenGuid, approvalUrl, rejectUrl);
                            }

                        }
                      }

                       

                           
                        
                     }



                    //LogApprovalHistory(RequestType, statusID,
                    //    "", DateTime.Now, "Approved");
                }

                // The travel request has been approved successfully
                // You can return a view or a message to indicate the success

                
                 ViewData["requestNumber"] = requestNumber;
                return View();
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in request number '{requestNumber}' in method '{methodName}'";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                // Handle any exceptions that may occur during the approval process
                // You can log the error or return an error view
                return View("ErrorView");
            }
        }


        [Route("RequestApproval/RequestNotFound")]
        public IActionResult RequestNotFound()
        {
            return View();
        }

            [Route("RequestApproval/RejectRequest")]
        public IActionResult RejectRequest(string token)
        {
            try
            {
               
                return View();
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}'";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                // Handle any exceptions that may occur during the rejection process
                // You can log the error or return an error view
                return View("ErrorView");
            }
        }

        [HttpPost]
        public IActionResult RejectCurrentRequest(string token, string comment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    // Create a command to call the stored procedure
                    using (SqlCommand command = new SqlCommand("trvl_SPRejectTravelRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Token", token); 
                        command.Parameters.AddWithValue("@Comment", comment);
                        command.Parameters.Add(new SqlParameter("@IsNotFound", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Status_ID", SqlDbType.Int) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@RequestOwnerName", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@RequestOwnerEmail", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@RequestNumber", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@TravelRequestId", SqlDbType.Int) { Direction = ParameterDirection.Output });


                        command.ExecuteNonQuery();

                       

                       

                        // Check if the request was not found
                        bool isNotFound = (bool)command.Parameters["@IsNotFound"].Value;

                        if (isNotFound)
                        {
                            return Json("2");
                        }
                        else
                        {
                            var statusID = (int)command.Parameters["@Status_ID"].Value;
                            string RequestOwnerName = (string)command.Parameters["@RequestOwnerName"].Value;
                            string RequestOwnerEmail = (string)command.Parameters["@RequestOwnerEmail"].Value;
                            string RequestNumber = (string)command.Parameters["@RequestNumber"].Value;
                            
                                 var TravelRequestId = (int)command.Parameters["@TravelRequestId"].Value;

                            int RequestType = 1;
                            string strRequType = "Travel Request";
                            string emailsubject = "Travel Request Rejected";
                            if (token.Contains("Expense-"))//Expense token
                            {
                                RequestType = 2;
                                emailsubject = "Expense Request Rejected";
                                strRequType = "Expense Request";
                            }

                            SendEmailController eml1 = new SendEmailController(_configuration);

                            eml1.RejectRequest(comment, RequestOwnerName,
                                RequestOwnerEmail, emailsubject, 
                                TravelRequestId, strRequType);


                            //   LogApprovalHistory(RequestType, statusID,
                            //"", DateTime.Now, "Rejected");
                        }
                    }
                }
                return Json("1");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}'";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                // Handle any exceptions that may occur during the rejection process
                // You can log the error or return an error view
                return View("ErrorView");
            }

        }
        public List<AccountsPersonnel> GetAccountsDepartmentPersonnel()
        {
            List<AccountsPersonnel> personnelList = new List<AccountsPersonnel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))

            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_SPGetAccountsDepartmentEmployees", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccountsPersonnel personnel = new AccountsPersonnel
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

        public List<DirectorPersonnel> GetDirectorDepartmentPersonnel()
        {
            List<DirectorPersonnel> personnelList = new List<DirectorPersonnel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_SPGetDirectorDepartmentPersonnel", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DirectorPersonnel personnel = new DirectorPersonnel
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
        
      public List<InsurancePersonnel> GetInsuranceDepartmentPersonnel()
        {
            List<InsurancePersonnel> personnelList = new List<InsurancePersonnel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_SPGetGetInsuranceDepartmentPersonnel", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InsurancePersonnel personnel = new InsurancePersonnel
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
        public class InsurancePersonnel
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
        }
        public class TravelDeskPersonnel
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
        }
        public class DirectorPersonnel
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
        }

        public class AdminPersonnel
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
        }
        public class AccountsPersonnel
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
        }
        public void UpdatePendingTravelRequest(int requestId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand command = new SqlCommand("trvl_SPUpdatePendingTravelRequest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the stored procedure
                    command.Parameters.Add(new SqlParameter("@Requestid", requestId));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        string exceptionMessage = $"Exception in method '{methodName}'";

                        // Log or rethrow the exception with the updated message
                        var errorLogger = new ErrorLogger(_configuration);
                        errorLogger.LogError(ex, exceptionMessage);
                        // Handle any exceptions that may occur
                        throw ex;
                    }
                }
            }
        }

        public void SPUpdateStatusPendingWithAdmin(int requestId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand command = new SqlCommand("trvl_SPUpdateStatusPendingWithAdmin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the stored procedure
                    command.Parameters.Add(new SqlParameter("@Requestid", requestId));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        string exceptionMessage = $"Exception in method '{methodName}'";

                        // Log or rethrow the exception with the updated message
                        var errorLogger = new ErrorLogger(_configuration);
                        errorLogger.LogError(ex, exceptionMessage);
                        // Handle any exceptions that may occur
                        throw ex;
                    }
                }
            }
        }

        public void SPUpdateStatusTicketIsBooked(int requestId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand command = new SqlCommand("trvl_SPUpdateStatusForTicketBooked", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the stored procedure
                    command.Parameters.Add(new SqlParameter("@Requestid", requestId));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        string exceptionMessage = $"Exception in method '{methodName}'";

                        // Log or rethrow the exception with the updated message
                        var errorLogger = new ErrorLogger(_configuration);
                        errorLogger.LogError(ex, exceptionMessage);
                        // Handle any exceptions that may occur
                        throw ex;
                    }
                }
            }
        }

        private void LogApprovalHistory(int itemType, int statusId, string updatedBy, DateTime updateDate, string action)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    using (SqlCommand logCommand = new SqlCommand("trvl_SP_SaveApprovalHistory", connection))
                    {
                        logCommand.CommandType = CommandType.StoredProcedure;
                        logCommand.Parameters.AddWithValue("@ItemType", itemType);
                        logCommand.Parameters.AddWithValue("@StatusId", statusId);
                        logCommand.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                        logCommand.Parameters.AddWithValue("@UpdateDate", updateDate);
                        logCommand.Parameters.AddWithValue("@Action", action);
                      
                            connection.Open();
                            // Execute the stored procedure for logging
                            logCommand.ExecuteNonQuery();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}'";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                // Handle any exceptions that may occur
                throw ex;
            }
        }

        private void LogApprovalData(int travelRequestId, int statusID, 
            string nameOfReceiver, bool isSentMailToTravelDesk,
            bool isAdvanceAmountMailToAccounts, bool isSentMailToAdmin,
            bool isSentMailToInsurance, string requestOwnerName, 
            string requestOwnerEmail, string requestNumber, 
            bool isMakeStatusBookedWhenbySelf, string token)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_sp_LogApprovalData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TravelRequestId", travelRequestId);
                    command.Parameters.AddWithValue("@StatusID", statusID);
                    command.Parameters.AddWithValue("@NameOfReceiver", nameOfReceiver);
                    command.Parameters.AddWithValue("@IsSentMailToTravelDesk", isSentMailToTravelDesk);
                    command.Parameters.AddWithValue("@IsAdvanceAmountMailToAccounts", isAdvanceAmountMailToAccounts);
                    command.Parameters.AddWithValue("@IsSentMailToAdmin", isSentMailToAdmin);
                    command.Parameters.AddWithValue("@IsSentMailToInsurance", isSentMailToInsurance);
                    command.Parameters.AddWithValue("@RequestOwnerName", requestOwnerName);
                    command.Parameters.AddWithValue("@RequestOwnerEmail", requestOwnerEmail);
                    command.Parameters.AddWithValue("@RequestNumber", requestNumber);
                    command.Parameters.AddWithValue("@IsMakeStatusBookedWhenbySelf", isMakeStatusBookedWhenbySelf);
                    command.Parameters.AddWithValue("@LogToken", token); // You need to generate the log token

                    command.ExecuteNonQuery();
                }
            }
        }


    }
}

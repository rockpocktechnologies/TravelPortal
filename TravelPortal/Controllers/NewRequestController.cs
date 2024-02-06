using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Net;
using TravelPortal.Models;
using TravelPortal.Classes;
using static TravelPortal.Controllers.RequestApprovalController;
using static TravelPortal.Classes.CommonFunctions;
using Microsoft.AspNetCore.Authorization;

namespace TravelPortal.Controllers
{
    public class NewRequestController : Controller
    {

        private readonly IConfiguration _configuration; // Inject your configuration if needed

        public NewRequestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

   //[Authorize]
        public IActionResult Index()
        {
            List<MultiCityData> lst = new List<MultiCityData>();
            NewRequestModel model = new NewRequestModel();
            string email = "";

            try
            {
                var nameClaim = User.FindFirst("Name");

                if (nameClaim != null && nameClaim.Subject != null)
                {
                    email = nameClaim.Subject.Name;

                    bool isForVolTravelEmail = CheckForVolTravelEmail(email);

                    // Your boolean flag is now set based on the condition
                    if (isForVolTravelEmail)
                    {
                        HttpContext.Session.SetString("isForVolTravelEmail", "true");
                    }

                    // Use userName as needed
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}': {ex.Message}";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
            }

            string isLocal = _configuration["EmailSettings:IsLocal"];

            if (isLocal == "true")
            {
                ViewData["LoggedInEmail"] = "approver5@afconsinfra.onmicrosoft.com";

            }
            else
            {
                ViewData["LoggedInEmail"] = email;
            }


            model.MultiCityData = lst;
            return View(model);
        }

        private bool CheckForVolTravelEmail(string email)
        {
            // Assuming you want to perform a case-insensitive check
            return email.EndsWith("@forvoltravel.com", StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost]
        public IActionResult SubmitTravelRequest([FromBody] NewRequestModel model)
        {
            int newTravelRequestIdNew = 0;

            try
            {

                //string ManagerEmpNo = HttpContext.Session.GetString("SManagerEmpNo");
                //string Manager2EmpNo = HttpContext.Session.GetString("SManager2EmpNo");
                string SEmployeeEmailId = HttpContext.Session.GetString("SEmployeeEmailId");
                string SName = HttpContext.Session.GetString("SName");

                string newRequestToken = "";
                bool IsDirectorApprovalNeeded = false;
                List<DirectorPersonnel> MDDirectorPersonnelList = new List<DirectorPersonnel>();
                CommonFunctions cmn = new CommonFunctions(_configuration);

                DataTable multiCityDataTable = ConvertMultiCityDataToDataTable(model.MultiCityData, cmn);

                using (SqlConnection connection =
               new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_SPSaveTravelRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (string.IsNullOrEmpty(model.strTravelMode))
                        {
                            command.Parameters.AddWithValue("@Travel_Mode", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Travel_Mode", Convert.ToInt32(model.strTravelMode));
                        }
                        command.Parameters.AddWithValue("@Ticket_Booked_By", model.strTicketBookedBy);
                        if (string.IsNullOrEmpty(model.strTravelDestination))
                        {
                            command.Parameters.AddWithValue("@Travel_Destination", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Travel_Destination", Convert.ToInt32(model.strTravelDestination));
                        }
                        if (string.IsNullOrEmpty(model.strdivTravelType))
                        {
                            command.Parameters.AddWithValue("@Travel_Type", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Travel_Type", Convert.ToInt32(model.strdivTravelType));
                        }
                        //if (string.IsNullOrEmpty(model.strselectGender))
                        //{
                        //    command.Parameters.AddWithValue("@Emp_Gender", DBNull.Value);
                        //}
                        //else
                        //{
                        //    command.Parameters.AddWithValue("@Emp_Gender", Convert.ToInt32(model.strselectGender));
                        //}
                        command.Parameters.AddWithValue("@Departure_City", model.strtravelFrom);
                        command.Parameters.AddWithValue("@Destination_City", model.strtravelTo);

                        command.Parameters.AddWithValue("@Departure_Date",
                            cmn.ConvertDateFormatddmmyytommddyyDuringSave(model.strtravelDepartureDate));
                        command.Parameters.AddWithValue("@Return_Date",
                            cmn.ConvertDateFormatddmmyytommddyyDuringSave(model.strttravelReturnDate));
                        

                        if (string.IsNullOrEmpty(model.strRequestedFor))
                        {
                            command.Parameters.AddWithValue("@Requested_For", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Requested_For", model.strRequestedFor);
                        }
                        if (string.IsNullOrEmpty(model.strEmpType))
                        {
                            command.Parameters.AddWithValue("@Emp_Type", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Emp_Type", Convert.ToInt32(model.strEmpType));
                        }
                        if (string.IsNullOrEmpty(model.stremployeeNo))
                        {
                            command.Parameters.AddWithValue("@Emp_No", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Emp_No", Convert.ToInt32(model.stremployeeNo));
                        }
                        command.Parameters.AddWithValue("@Emp_Name", model.stremployeeName);
                        command.Parameters.AddWithValue("@Emp_Grade", model.strgrade);
                        command.Parameters.AddWithValue("@Emp_Position", model.strposition);
                        command.Parameters.AddWithValue("@Emp_Department", model.strdepartment);
                        command.Parameters.AddWithValue("@Emp_Location", model.strlocation);
                        command.Parameters.AddWithValue("@Emp_Mobile", model.strmobileNumber);
                        if (string.IsNullOrEmpty(model.strage))
                        {
                            command.Parameters.AddWithValue("@Emp_Age", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Emp_Age", Convert.ToInt32(model.strage));
                        }
                        if (string.IsNullOrEmpty(model.strselectGender))
                        {
                            command.Parameters.AddWithValue("@Emp_Gender", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Emp_Gender", Convert.ToInt32(model.strselectGender));
                        }
                        if (string.IsNullOrEmpty(model.strselectPurpose))
                        {
                            command.Parameters.AddWithValue("@Travel_Purpose", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Travel_Purpose", Convert.ToInt32(model.strselectPurpose));
                        }
                        command.Parameters.AddWithValue("@Travel_Days", model.strtravelDays);
                        //command.Parameters.AddWithValue("@Manager1", ManagerEmpNo);
                        //command.Parameters.AddWithValue("@Manager2", Manager2EmpNo);
                        command.Parameters.AddWithValue("@Director", model.strdirector);
                        if (string.IsNullOrEmpty(model.stradvanceAmount))
                        {
                            command.Parameters.AddWithValue("@Requested_Advance_Amt", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Requested_Advance_Amt", Convert.ToInt32(model.stradvanceAmount));
                        }
                        if (string.IsNullOrEmpty(model.selectCurrency))
                        {
                            command.Parameters.AddWithValue("@Currency", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Currency", Convert.ToInt32(model.selectCurrency));
                        }

                        if (string.IsNullOrEmpty(model.strselectProjectCode))
                        {
                            command.Parameters.AddWithValue("@Emp_Project", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Emp_Project", Convert.ToInt32(model.strselectProjectCode));
                        }
                        command.Parameters.AddWithValue("@Emp_Comments", model.strcomments);
                        command.Parameters.AddWithValue("@RequestToken", model.strRequestToken);
                        command.Parameters.AddWithValue("@IsSubmitted", model.isSubmitted);
                        command.Parameters.Add(new SqlParameter("@MulticityDetails", SqlDbType.Structured)
                        {
                            TypeName = "MulticityTravelDetails", // Assuming dbo.MultiCityType is the type name in your SQL database
                            Value = multiCityDataTable
                        });

                        command.Parameters.AddWithValue("@NameonPassport", model.strNameonPassport);
                        command.Parameters.AddWithValue("@PassportNumber", model.strPassportNumber);
                        command.Parameters.AddWithValue("@PassportIssueDate",
                        cmn.ConvertDateFormatddmmyytommddyyDuringSave(model.strPassportIssueDate));
                        command.Parameters.AddWithValue("@PassportExpiryDate",
                        cmn.ConvertDateFormatddmmyytommddyyDuringSave(model.strPassportExpiryDate));

                        command.Parameters.AddWithValue("@NameofNominee", model.strNameofNominee);
                        command.Parameters.AddWithValue("@RelationwithNominee", model.strRelationwithNominee);
                        command.Parameters.AddWithValue("@DOBofNominee",
                        cmn.ConvertDateFormatddmmyytommddyyDuringSave(model.strDOBofNominee));
                        command.Parameters.AddWithValue("@GenderofNominee", model.strGenderofNominee);

                        var newRequestIdParam = command.Parameters.Add("@NewTravelRequestID", SqlDbType.Int);
                        newRequestIdParam.Direction = ParameterDirection.Output;

                        var newRequestTokenParam = command.Parameters.Add("@NewRequestToken", SqlDbType.NVarChar, 255); // Replace 255 with the appropriate size
                        newRequestTokenParam.Direction = ParameterDirection.Output;

                        var newIsDirectorApprovalNeededParam = command.Parameters.Add("@IsDirectorApprovalNeeded", SqlDbType.Bit); // Replace 255 with the appropriate size
                        newIsDirectorApprovalNeededParam.Direction = ParameterDirection.Output;


                        var Manager1Name = command.Parameters.Add("@Manager1Name", SqlDbType.NVarChar, 255); // Replace 255 with the appropriate size
                        Manager1Name.Direction = ParameterDirection.Output;

                        var Manager1Email = command.Parameters.Add("@Manager1Email", SqlDbType.NVarChar, 255); // Replace 255 with the appropriate size
                        Manager1Email.Direction = ParameterDirection.Output;
                        
                        try
                        {


                            command.ExecuteNonQuery();
                            // Retrieve the newly created travel request ID
                        

                             newRequestToken = (string)newRequestTokenParam.Value;
                            IsDirectorApprovalNeeded = (bool)newIsDirectorApprovalNeededParam.Value;

                            if (model.isSubmitted)
                            {

                                int newTravelRequestId = (int)newRequestIdParam.Value;
                                newTravelRequestIdNew = newTravelRequestId;

                                if (Manager1Email.Value != System.DBNull.Value)
                                {
                                    string Manager1EmailNew = (string)Manager1Email.Value;

                                    string Manager1NameNew = (string)Manager1Name.Value;

                                    TokenService tkn = new TokenService(_configuration);
                                    string tokenGuid = tkn.CreateNewTravelRequestToken(newTravelRequestId, 1);
                                    var approvalUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/ApproveRequest"}";
                                    var rejectUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/RejectRequest"}";

                                    //string mgrName = HttpContext.Session.GetString("SManager");
                                    //string mgrEmail = HttpContext.Session.GetString("SManagerEmail");

                                    if (Manager1EmailNew != null)
                                    {
                                        SendEmailController eml = new SendEmailController(_configuration);

                                        eml.SendApprovalEmail(newTravelRequestId, Manager1NameNew, Manager1EmailNew,
                                        tokenGuid, approvalUrl, rejectUrl);


                                    }
                                }
                                

                                if (model.isCreateMode)
                                {
                                    SendEmailController eml1 = new SendEmailController(_configuration);

                                    eml1.SendNewRequestEmail(newTravelRequestId, SName, SEmployeeEmailId);

                                }

                            }
           
                            if (model.strTravelDestination == "2")
                            {
                                MDDirectorPersonnelList = 
                                    GetManagingDirectorDepartmentPersonnel();

                               
                            }
                        }
                        catch (Exception ex)
                        {
                            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            string exceptionMessage = $"Exception in method '{methodName}'";

                            // Log or rethrow the exception with the updated message
                            var errorLogger = new ErrorLogger(_configuration);
                            errorLogger.LogError(ex, exceptionMessage);
                       
                            // Handle exceptions, e.g., log or display an error message
                            return Json("0");
                        }
                    }
                }
                return Json(new[] { new
                {
                    newRequestToken = newRequestToken,
                    IsDirectorApprovalNeeded = IsDirectorApprovalNeeded,
                    MDDirectorPersonnelList = MDDirectorPersonnelList
                }});
                //return Ok(newRequestToken.ToString());
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}' and request id = '{newTravelRequestIdNew}' ";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                return Json("0");
            }
        }

        [HttpGet]
        [Route("NewRequest/GetMasterData")]
            public JsonResult GetMasterData()
        {
            using (SqlConnection connection =
               new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                // Create a SQL command to fetch all the master data in a single query
                string sqlQuery = "SELECT * FROM trvl_tmTravelMode; SELECT * FROM trvl_Ticket_Booked_By; SELECT * FROM trvl_Travel_Destination; SELECT * FROM trl_Travel_Type";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Create a data reader to read the results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var masterData = new MasterData();

                        // Read the first result set (trvl_tmTravelMode)
                        while (reader.Read())
                        {
                            var travelMode = new TravelMode
                            {
                                Id = reader.GetInt32(0), // Assuming the first column is the ID
                                Name = reader.GetString(1), // Assuming the second column is the Name
                                imageUrl = reader.GetString(2),                  // Add more properties as needed
                            };
                            masterData.TravelModes.Add(travelMode);
                        }

                        // Move to the next result set (trvl_Ticket_Booked_By)
                        reader.NextResult();

                        // Read the second result set (trvl_Ticket_Booked_By)
                        while (reader.Read())
                        {
                            var ticketBookedBy = new TicketBookedBy
                            {
                                Id = reader.GetInt32(0), // Assuming the first column is the ID
                                Name = reader.GetString(1), // Assuming the second column is the Name
                                                            // Add more properties as needed
                            };
                            masterData.TicketBookedBys.Add(ticketBookedBy);
                        }

                        // Repeat this pattern for the remaining result sets (trvl_Travel_Destination, trl_Travel_Type)

                        return Json(masterData);
                    }
                }
            }
        }

        [HttpGet]
        [Route("NewRequest/GetEmployeeData")]
        public IActionResult GetEmployeeData(string employeeEmailId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");

                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_GetEmployeeData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmployeeEmailId", employeeEmailId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var employeeData = new
                                {
                                    EmployeeNo = reader["EmployeeNo"],
                                    Name = reader["Name"],
                                    Grade = reader["Grade"],
                                    Position = reader["Position"],
                                    Department = reader["Department"],
                                    SPSLocation = reader["SPSLocation"],
                                    MobileNumber = reader["MobileNumber"],
                                    DateOfBirth = reader["DateOfBirth"],
                                    Gender = reader["Gender"],
                                    Manager = reader["Manager"],
                                    Manager2 = reader["Manager2"],
                                    ManagerEmail = reader["ManagerEmailID"],
                                    ManagerEmpNo = reader["ManagerEmpNo"],
                                    Manager2EmpNo = reader["Manager2EmpNo"],
                                    IsGradeFindInEligibility = reader["IsGradeFindInEligibility"],
                                    IsGradeFindInPolicyDetails = reader["IsGradeFindInPolicyDetails"],
                                    IsBlockNewTicket = reader["IsBlockNewTicket"]
                                };



                                HttpContext.Session.SetString("SEmployeeNo", employeeData.EmployeeNo.ToString());
                                HttpContext.Session.SetString("SName", employeeData.Name.ToString());
                                HttpContext.Session.SetString("SGrade", employeeData.Grade.ToString());
                                HttpContext.Session.SetString("SPosition", employeeData.Position.ToString());
                                HttpContext.Session.SetString("SDepartment", employeeData.Department.ToString());
                                HttpContext.Session.SetString("SSPSLocation", employeeData.SPSLocation.ToString());
                                HttpContext.Session.SetString("SMobileNumber", employeeData.MobileNumber.ToString());
                                HttpContext.Session.SetString("SDateOfBirth", employeeData.DateOfBirth.ToString());
                                HttpContext.Session.SetString("SGender", employeeData.Gender.ToString());
                                HttpContext.Session.SetString("SManager", employeeData.Manager.ToString());
                                HttpContext.Session.SetString("SManager2", employeeData.Manager2.ToString());
                                HttpContext.Session.SetString("SManagerEmail", employeeData.ManagerEmail.ToString());
                                HttpContext.Session.SetString("SEmployeeEmailId", employeeEmailId);
                                HttpContext.Session.SetString("SManagerEmpNo", employeeData.ManagerEmpNo.ToString());
                                HttpContext.Session.SetString("SManager2EmpNo", employeeData.Manager2EmpNo.ToString());

                                bool IsEmployeeTravelDesk = CheckLoggedInPersonIsTravelDesk(employeeData.EmployeeNo.ToString());

                                //if (employeeEmailId == "approver4@afconsinfra.onmicrosoft.com")
                                //{
                                //    HttpContext.Session.SetString("SIsTravelDesk", "true");
                                //}

                                var response = new
                                {
                                    EmployeeData = employeeData,
                                    IsEmployeeTravelDesk = IsEmployeeTravelDesk
                                };

                                return Json(response);

                                
                            }
                        }
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
                return Json(new { error = ex.Message });
            }

            return Json(null);
        }

        [HttpGet]
        [Route("NewRequest/GetEmpDataByEmpNo")]
        public IActionResult GetEmpDataByEmpNo(string EmployeeNumber,
            string strRequestToken)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_GetEmployeeDataByEmpNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmployeeNumber", EmployeeNumber);
                        command.Parameters.AddWithValue("@RequestToken", strRequestToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var employeeData = new
                                {
                                    EmployeeNo = reader["EmployeeNo"],
                                    Name = reader["Name"],
                                    Grade = reader["Grade"],
                                    Position = reader["Position"],
                                    Department = reader["Department"],
                                    SPSLocation = reader["SPSLocation"],
                                    MobileNumber = reader["MobileNumber"],
                                    DateOfBirth = reader["DateOfBirth"],
                                    Gender = reader["Gender"],
                                    Manager = reader["Manager"],
                                    Manager2 = reader["Manager2"],
                                    ManagerEmail = reader["ManagerEmailID"],
                                    ManagerEmpNo = reader["ManagerEmpNo"],
                                    Manager2EmpNo = reader["Manager2EmpNo"],
                                    IsGradeFindInEligibility = reader["IsGradeFindInEligibility"],
                                    IsGradeFindInPolicyDetails = reader["IsGradeFindInPolicyDetails"],
                                    IsDirectorApprovalNeeded = reader["IsDirectorApprovalNeeded"]
                                };




                                bool IsEmployeeTravelDesk = false;

                                //if (employeeEmailId == "approver4@afconsinfra.onmicrosoft.com")
                                //{
                                //    HttpContext.Session.SetString("SIsTravelDesk", "true");
                                //}

                                var response = new
                                {
                                    EmployeeData = employeeData,
                                    IsEmployeeTravelDesk = IsEmployeeTravelDesk
                                };

                                return Json(response);


                            }
                        }
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
                return Json(new { error = ex.Message });
            }

            return Json(null);
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


        [HttpPost]
        public JsonResult GetCities(int Destination)
        {
            
            
            List<string> cityNames = new List<string>();

            using (SqlConnection connection =
               new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                string query = "SELECT CityName FROM " +
                    "trvl_tmTravelCityDetails WHERE TravelDestination = @destination";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@destination", Destination);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cityNames.Add(reader["CityName"].ToString());
                        }
                    }
                }
            }

            return Json(cityNames);
        }

        [HttpGet]
        public IActionResult BindProjects()
        {
            List<Project> projects = new List<Project>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();

                using (var command = new SqlCommand("trvl_SPGetProjects", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var project = new Project
                            {
                                ProjectId = Convert.ToInt32(reader["ID"]),
                                ProjectName = reader["Project_Name"].ToString()
                            };
                            projects.Add(project);
                        }
                    }
                }
            }

            return Json(projects);
        }

        public List<DirectorPersonnel> GetManagingDirectorDepartmentPersonnel()
        {
            List<DirectorPersonnel> personnelList = new List<DirectorPersonnel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))

            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_SPGetManagingDirectorDepartmentEmployees", connection))
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

        public class Project
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
        }
        public class DirectorPersonnel
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
        }


        //public string CreateNewTravelRequestToken(int newTravelRequestId)
        //{

        //    // Generate a unique token
        //    string uniqueToken = GenerateUniqueToken();
        //    string connectionString = _configuration.GetConnectionString("ConnectionString");
        //    // Store the token in the trvl_tblApprovalTokens table
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (var command = new SqlCommand("INSERT INTO trvl_tblApprovalTokens (Token, TravelRequestId, isActive) VALUES (@Token, @TravelRequestId, @isActive)", connection))
        //        {
        //            command.Parameters.AddWithValue("@Token", uniqueToken);
        //            command.Parameters.AddWithValue("@TravelRequestId", newTravelRequestId); // The ID of the newly created travel request
        //            command.Parameters.AddWithValue("@isActive", true); // 
        //            command.ExecuteNonQuery();
        //        }
        //    }

        //    return uniqueToken;
        //}


        //public string GenerateUniqueToken()
        //{
        //    // Generate a random token, e.g., using Guid
        //    return Guid.NewGuid().ToString();
        //}

        [HttpGet]
        public JsonResult GetCurrencies()
        {
            List<Currency> currencies = new List<Currency>();

            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("trvl_SPGetCurrencies", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var currency = new Currency
                            {
                                CurrencyId = Convert.ToInt32(reader["ID"]),
                                CurrencyCode = reader["ShortName"].ToString(),
                                ExchangeRate = reader["ExchangeRate"] == DBNull.Value ? "" : reader["ExchangeRate"].ToString(),
                            };
                            currencies.Add(currency);
                        }
                    }
                }
            }

            return Json(currencies);
        }

        [HttpGet]
        public JsonResult GetPurposes()
        {
            List<Purpose> purposes = new List<Purpose>();

            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("trvl_SPGetPurposes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purpose = new Purpose
                            {
                                PurposeId = Convert.ToInt32(reader["ID"]),
                                PurposeName = reader["Title"].ToString()
                            };
                            purposes.Add(purpose);
                        }
                    }
                }
            }

            return Json(purposes);
        }

        [HttpGet]
        public JsonResult GetDirectors()
        {
            List<Director> directors = new List<Director>();

            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("trvl_SPGetDirectorDepartmentPersonnel", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var director = new Director
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString()
                            };
                            directors.Add(director);
                        }
                    }
                }
            }

            return Json(directors);
        }
        public class TravelMode
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public string imageUrl { get; set; }
            
            // Add more properties as needed
        }

        public class Director
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
        }

        public class TicketBookedBy
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // Add more properties as needed
        }

        public class TravelDestination
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // Add more properties as needed
        }

        public class TravelType
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // Add more properties as needed
        }

        public class MasterData
        {
            public List<TravelMode> TravelModes { get; set; } = new List<TravelMode>();
            public List<TicketBookedBy> TicketBookedBys { get; set; } = new List<TicketBookedBy>();
            public List<TravelDestination> TravelDestinations { get; set; } = new List<TravelDestination>();
            public List<TravelType> TravelTypes { get; set; } = new List<TravelType>();
        }

        public class Currency
        {
            public int CurrencyId { get; set; }
            public string CurrencyCode { get; set; }

            public string ExchangeRate { get; set; }
        }

        public class Purpose
        {
            public int PurposeId { get; set; }
            public string PurposeName { get; set; }
        }

        public DataTable ConvertMultiCityDataToDataTable(List<MultiCityData> multiCityData,CommonFunctions cmn)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SourceCity", typeof(string));
            dt.Columns.Add("DestinationCity", typeof(string));
            dt.Columns.Add("DepartureDate", typeof(string));

            foreach (var cityData in multiCityData)
            {
                dt.Rows.Add(cityData.travelFrom, cityData.travelTo,
                    cmn.ConvertDateFormatddmmyytommddyyDuringSave(cityData.travelDepartureDate));
            }

            return dt;
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using TravelPortal.Classes;
using TravelPortal.Models;
using static TravelPortal.Controllers.RequestApprovalController;

namespace TravelPortal.Controllers
{
    public class AddExpenseController : Controller
    {
        private string _connectionString; // Your database connection string

        private readonly IConfiguration _configuration;

        public AddExpenseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(string RequestToken)
        {

            List<GetTravelEpense> expenses = GetExpenseData(RequestToken);

            // Pass the list of expenses as the model data to the view
            return View(expenses);
        }


        [HttpPost]
        public IActionResult GetDateRange(string requestToken)
        {
            try
            {
                using (SqlConnection connection =
                    new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_spGetDateRangeForRequestToken", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@RequestToken", requestToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime departureDate = reader.GetDateTime(0);
                                DateTime maxReturnDate = reader.GetDateTime(1);

                                return Ok(new { DepartureDate = departureDate, MaxReturnDate = maxReturnDate });
                            }
                        }
                    }
                }

                return NotFound(); // If no records are found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public List<TravelCategory> GetTravelCategories()
        {
            List<TravelCategory> categories = new List<TravelCategory>();
            try
            {
                

                using (SqlConnection connection =
                    new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("trvl_SPGetTravelCategories", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TravelCategory category = new TravelCategory
                                {
                                    ID = (int)reader["ID"],
                                    Title = reader["title"].ToString(),
                                    ForInternational = (bool)reader["ForInternational"],
                                    ForDomestic = (bool)reader["ForDomestic"]
                                };
                                categories.Add(category);
                            }
                        }
                    }
                }

               
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}': {ex.Message}";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                return categories;
            }
            return categories;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenses(List<TrvlExpense> expenses)
        {

            try
            {
               // bool isexpensesubmitted = false;

                int totalExpenses = expenses.Count;

                // Iterate through the list of expenses
                bool isSubmit = false;
                int RequestID = 0;
                int i = 0;
                int newindex = 0;
                string RequestNumber = "";
                bool IsSendToManagerForApproval = true;
                bool IsFromAccounts = false;

                CommonFunctions cmn = new CommonFunctions(_configuration);

                foreach (var expense in expenses)
                {
                    newindex = newindex + 1;

                    if(newindex == totalExpenses)
                    {
                        if (expenses[newindex - 1 ].isSubmit == true)// array index start from 0
                        {
                            isSubmit = true;
                        }
                       // isexpensesubmitted = true;
                    }

                    byte[] fileData = null;
                    string filename = null;

                    if (expense.FileData != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await expense.FileData.CopyToAsync(stream);

                            // The 'stream' variable now contains the file data as a stream
                            // You can convert it to byte[] or store it as needed
                            // For example:
                            var fileBytes = stream.ToArray();
                            // Convert base64 data to a byte array
                            fileData = fileBytes;
                            filename = expense.FileData.FileName;

                        }
                    }
                    try
                    {

                        int expenseID = 0;
                        if (expense.expenseID == null)
                        {
                            expense.expenseID = "";
                        }

                        if (expense.expenseID != "" && expense.expenseID != "0")
                        {
                            expenseID = Convert.ToInt32(expense.expenseID);
                        }

                        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                        {
                            connection.Open();



                            if (expense.ByCompany == null)
                            {
                                expense.ByCompany = "";
                            }

                            if (expense.Comment == null)
                            {
                                expense.Comment = "";
                            }
                            using (var command = new SqlCommand("trvl_SPInsertExpense", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@ExpenseID", expense.expenseID);
                                command.Parameters.AddWithValue("@RequestToken", expense.RequestToken);
                                command.Parameters.AddWithValue("@ExpenseType", expense.ExpenseType);
                                command.Parameters.AddWithValue("@FromDate",
                                    cmn.ConvertDateFormatddmmyytommddyyDuringSave(expense.FromDate));
                                command.Parameters.AddWithValue("@ToDate",
                                    cmn.ConvertDateFormatddmmyytommddyyDuringSave(expense.ToDate));
                                command.Parameters.AddWithValue("@Currency", expense.Currency);
                                command.Parameters.AddWithValue("@ByCompany", expense.ByCompany);
                                command.Parameters.AddWithValue("@ByEmployee", expense.ByEmployee);
                                command.Parameters.AddWithValue("@Comment", expense.Comment);
                                command.Parameters.AddWithValue("@IsSubmit", isSubmit);
                                command.Parameters.AddWithValue("@IsSendToManagerForApproval", expense.isSendToManagerForApproval);

                                //command.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary) { Value = fileData });
                                if (fileData != null)
                                {
                                    command.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary) { Value = fileData });
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@FileData", SqlBinary.Null);
                                }
                                //command.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary) { Value = fileData });
                                if (fileData != null)
                                {
                                    command.Parameters.AddWithValue("@FileName", filename);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@FileName", DBNull.Value);
                                }

                                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                command.Parameters.AddWithValue("@CreatedBy", "1");
                                command.Parameters.AddWithValue("@IsUpdateAttachment", expense.IsUpdateAttachment);
                                command.Parameters.AddWithValue("@FinalAmount", expense.strFinalAmount);

                                command.ExecuteNonQuery();
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
                        return Json("0");

                    }



                    i = i + 1;


                    if (isSubmit)
                    {


                       // isSubmit = true;
                        RequestID = Convert.ToInt32(expense.strNumber);
                        RequestNumber = expense.RequestNumber;

                    }

                    if(expense.isSendToManagerForApproval == true)
                    {
                        IsSendToManagerForApproval = true;
                    }
                    else
                    {
                        IsSendToManagerForApproval = false;
                    }

                    if (expense.isFromAccounts == true)
                    {
                        IsFromAccounts = true;
                    }
                    else
                    {
                        IsFromAccounts = false;
                    }
                }

                // Process other expense properties as needed


                if (isSubmit == true && IsFromAccounts == false)
                {//send Approval email
                 //int newTravelRequestId = (int)newRequestIdParam.Value;

                    string bodyHtml = "";
                    foreach (var expense in expenses)
                    {


                        bodyHtml += "<tr>";
                        bodyHtml += "<td><strong>Expense Type</strong></td>";
                        bodyHtml += "<td>" + expense.strExpenseTypeText + "</td>";
                        bodyHtml += "</tr>";
                        bodyHtml += "<tr>";
                        bodyHtml += "<td><strong>From Date</strong></td>";
                        bodyHtml += "<td>" + expense.FromDate + "</td>";
                        bodyHtml += "</tr>";
                        bodyHtml += "<tr>";
                        bodyHtml += "<td><strong>To Date</strong></td>";
                        bodyHtml += "<td>" + expense.ToDate + "</td>";
                        bodyHtml += "</tr>";
                        bodyHtml += "<tr>";
                        bodyHtml += "<td><strong>By Employee</strong></td>";
                        bodyHtml += "<td>" + expense.ByEmployee + "</td>";
                        bodyHtml += "</tr>";
                        bodyHtml += "<tr>";
                        bodyHtml += "<td><strong>Currency</strong></td>";
                        bodyHtml += "<td>" + expense.strCurrencyText + "</td>";
                        bodyHtml += "</tr>";

                    }


                    TokenService tkn = new TokenService(_configuration);
                    string tokenGuid = tkn.CreateNewTravelRequestToken(RequestID, 2);
                    var approvalUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/ApproveRequest"}";
                    var rejectUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{"/RequestApproval/RejectRequest"}";

                    if (IsSendToManagerForApproval)
                    {
                        string mgrName = HttpContext.Session.GetString("SManager");
                        string mgrEmail = HttpContext.Session.GetString("SManagerEmail");
                        string SName = HttpContext.Session.GetString("SName");
                        SendEmailController eml = new SendEmailController(_configuration);
                        eml.SendExpenseApprovalEmail(RequestID, mgrName, mgrEmail,
                            tokenGuid, approvalUrl, rejectUrl, bodyHtml, SName, RequestNumber);

                    }
                    else
                    {
                        List<AccountsPersonnel> accountsPersonnelList = GetAccountsDepartmentPersonnel();

                        if (accountsPersonnelList.Count > 0)
                        {

                            foreach (var personnel in accountsPersonnelList)
                            {


                                SendEmailController eml = new SendEmailController(_configuration);
                                eml.SendEmailToAccounForExpenseApproval(RequestID, personnel.EmployeeEmail,
                                    personnel.EmployeeName, bodyHtml,approvalUrl,rejectUrl, tokenGuid);

                            }
                        }
                    }

                }

                return Json("1");

            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}': {ex.Message}";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);
                return Json("0");
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

        public List<GetTravelEpense> GetExpenseData(string requestToken)
        {
            List<GetTravelEpense> expenses = new List<GetTravelEpense>();
            try
            {


               
                CommonFunctions cmn = new CommonFunctions(_configuration);

                using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (var command = new SqlCommand("trvl_SPGetExpenseData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@RequestToken", requestToken);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var expense = new GetTravelEpense
                                {
                                    strExpenseType = reader["strExpenseType"] == DBNull.Value ? "" : reader["strExpenseType"].ToString(),
                                    strFromDate = reader["strFromDate"] == DBNull.Value ? "" : cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strFromDate"].ToString()),
                                    strToDate = reader["strToDate"] == DBNull.Value ? "" : cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strToDate"].ToString()),
                                    strCurrency = reader["strCurrency"] == DBNull.Value ? "" : reader["strCurrency"].ToString(),
                                    strByCompany = reader["strByCompany"] == DBNull.Value ? "" : reader["strByCompany"].ToString(),
                                    strByEmployee = reader["strByEmployee"] == DBNull.Value ? "" : reader["strByEmployee"].ToString(),
                                    strComment = reader["strComment"] == DBNull.Value ? "" : reader["strComment"].ToString(),
                                    strfilename = reader["strfilename"] == DBNull.Value ? "" : reader["strfilename"].ToString(),
                                    expenseID = reader["expenseID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["expenseID"].ToString()),
                                    issubmitted = reader["issubmitted"] == DBNull.Value ? false : Convert.ToBoolean(reader["issubmitted"]),
                                    ExpenseToken = reader["ExpenseToken"] == DBNull.Value ? "" : reader["ExpenseToken"].ToString(),
                                    strNameEmp = reader["strNameEmp"] == DBNull.Value ? "" : reader["strNameEmp"].ToString(),
                                    EmpNumber = reader["EmpNumber"] == DBNull.Value ? "" : reader["EmpNumber"].ToString(),
                                    EmpGrade = reader["EmpGrade"] == DBNull.Value ? "" : reader["EmpGrade"].ToString(),
                                    EmpMobile = reader["EmpMobile"] == DBNull.Value ? "" : reader["EmpMobile"].ToString(),

                                    

                                };


                                expenses.Add(expense);
                            }
                        }
                    }
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
            return expenses; // Return the list of expenses
        }


        public JsonResult GetEligibility(string requestToken)
        {
            List<EligibilityInfo> eligibility = GetEligibilityData(requestToken);
            return Json(eligibility);
        }

        public List<EligibilityInfo> GetEligibilityData(string requestToken)
        {
            List<EligibilityInfo> eligibilityList = new List<EligibilityInfo>();


            try
            {
                using (SqlConnection connection =
                                new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_SPGetEligibility", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@RequestToken", requestToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EligibilityInfo eligibility = new EligibilityInfo
                                {
                                    Grade = reader["Grade"] == DBNull.Value ? string.Empty : reader["Grade"].ToString(),
                                    TravelType = reader["TravelType"] == DBNull.Value ? string.Empty : reader["TravelType"].ToString(),
                                    CategoryTitle = reader["CategoryTitle"] == DBNull.Value ? string.Empty : reader["CategoryTitle"].ToString(),
                                    Entitlement = reader["Entitlement"] == DBNull.Value ? string.Empty : reader["Entitlement"].ToString(),
                                    ClassTitle = reader["ClassTitle"] == DBNull.Value ? string.Empty : reader["ClassTitle"].ToString(),
                                    EffectiveFrom = reader["EffectiveFrom"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["EffectiveFrom"],
                                    EffectiveTo = reader["EffectiveTo"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["EffectiveTo"],
                                    Currency = reader["Currency"] == DBNull.Value ? 0 : (int)reader["Currency"],
                                    IsActive = reader["IsActive"] == DBNull.Value ? false : (bool)reader["IsActive"],
                                    CompanyAuthority = reader["CompanyAuthority"] == DBNull.Value ? string.Empty : reader["CompanyAuthority"].ToString(),
                                    CategoryID  = reader["CategoryID"] == DBNull.Value ? 0 : (int)reader["CategoryID"],
                                    ExchangeRate = reader["ExchangeRate"] == DBNull.Value ? "" : (string)reader["ExchangeRate"],
                                    TravelDays = reader["TravelDays"] == DBNull.Value ? 0 : (int)reader["TravelDays"],
                                    CityName = reader["CityName"] == DBNull.Value ? "" : (string)reader["CityName"]

                                };

                                eligibilityList.Add(eligibility);
                            }
                        }
                    }
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

            return eligibilityList;
        }

        public IActionResult DownloadAttachment(string expenseToken)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_SPGetExpenseAttachment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ExpenseToken", expenseToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                byte[] fileData = (byte[])reader["file_data"];
                                string fileName = reader["filename"].ToString();

                                // Send the file as a response
                                return File(fileData, "application/octet-stream", fileName);
                            }
                        }
                    }
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
            // Call the stored procedure to get attachment data

            // Return a not found response if no attachment is found
            return NotFound();
        }

        public class EligibilityInfo
        {
            public string Grade { get; set; }
            public string TravelType { get; set; }
            public string CategoryTitle { get; set; }

            public int CategoryID { get; set; }
            public string Entitlement { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime EffectiveTo { get; set; }
            public int Currency { get; set; }
            public bool IsActive { get; set; }
            public string CompanyAuthority { get; set; }
            public string ClassTitle { get; set; }

            public string ExchangeRate { get; set; }

            public int TravelDays { get; set; }

            
                public string CityName { get; set; }
        }

        //public void AddExpenses(List<TrvlExpense>  Expense)
        //{

        //    try
        //    {

        //        //using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        //        //{
        //        //    connection.Open();

        //        //    using (var command = new SqlCommand("trvl_SPInsertExpense", connection))
        //        //    {
        //        //        command.CommandType = CommandType.StoredProcedure;

        //        //        command.Parameters.AddWithValue("@TravelRequestId", Expense.TravelRequestId);
        //        //        command.Parameters.AddWithValue("@ExpenseType", Expense.ExpenseType);
        //        //        command.Parameters.AddWithValue("@FromDate", Expense.FromDate);
        //        //        command.Parameters.AddWithValue("@ToDate", Expense.ToDate);
        //        //        command.Parameters.AddWithValue("@Currency", Expense.Currency);
        //        //        command.Parameters.AddWithValue("@ByCompany", Expense.ByCompany);
        //        //        command.Parameters.AddWithValue("@ByEmployee", Expense.ByEmployee);
        //        //        command.Parameters.AddWithValue("@Comment", Expense.Comment);

        //        //        // Convert base64 data to a byte array
        //        //        byte[] fileData = Expense.FileData;
        //        //        command.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary) { Value = fileData });

        //        //        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
        //        //        command.Parameters.AddWithValue("@CreatedBy", "1");

        //        //        command.ExecuteNonQuery();
        //        //    }
        //        //}

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}


        public class TravelCategory
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public bool ForInternational { get; set; }
            public bool ForDomestic { get; set; }
        }

        [HttpPost]
        public IActionResult DeleteAttachment(int id)
        {
            try
            {
                // Call a method to update the database and set file_data and filename to null based on the ID using ADO.NET
                UpdateAttachmentInDatabase(id);

                return Json(new { success = true, message = "Attachment deleted successfully." });
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}': {ex.Message}";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);

                return Json(new { success = false, message = "Error deleting attachment.", error = ex.Message });
            }
        }

        private void UpdateAttachmentInDatabase(int id)
        {
            using (SqlConnection connection =
                    new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_spUpdateAttachmentExpense", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the stored procedure
                    command.Parameters.AddWithValue("@ID", id);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();
                }
            }
        }

        

             [HttpPost]
        public IActionResult DeleteExpenseRow(int id)
        {
            try
            {
                // Call a method to update the database and set file_data and filename to null based on the ID using ADO.NET
                DeleteRowfromDatabase(id);

                return Json(new { success = true, message = "Attachment deleted successfully." });
            }
            catch (Exception ex)
            {

                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                string exceptionMessage = $"Exception in method '{methodName}': {ex.Message}";

                // Log or rethrow the exception with the updated message
                var errorLogger = new ErrorLogger(_configuration);
                errorLogger.LogError(ex, exceptionMessage);

                return Json(new { success = false, message = "Error deleting attachment.", error = ex.Message });
            }
        }

        private void DeleteRowfromDatabase(int id)
        {
            using (SqlConnection connection =
                   new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_spDeleteRowForExpense", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the stored procedure
                    command.Parameters.AddWithValue("@ID", id);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPost]
    public ActionResult UpdateStatusToPendingWithAccounts(string requestToken)
    {
        try
        {
                // Call the stored procedure to update the status
                using (SqlConnection connection =
                       new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_sp_UpdateStatusToPendingWithAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RequestToken", requestToken);

                    // Add more parameters as needed

                    command.ExecuteNonQuery();
                }
            }

            // Return success, if needed
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            // Handle exceptions, log errors, etc.
            return Json(new { success = false, error = ex.Message });
        }
    }


        [HttpPost]
        public ActionResult UpdateStatusToCloseByAccount(string requestToken)
        {
            try
            {
                // Call the stored procedure to update the status
                using (SqlConnection connection =
                       new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_sp_UpdateStatusToCloseByAccount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@RequestToken", requestToken);

                        // Add more parameters as needed

                        command.ExecuteNonQuery();
                    }
                }

                // Return success, if needed
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetEmployeeInformationforexpenseScreen(string requestToken)
        {
            EmployeeInfo employeeInfo = null;

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("trvl_SPGetEmployeeInformationforexpenseScreen", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RequestToken", requestToken);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            employeeInfo = new EmployeeInfo
                            {
                                EmpName = reader["Emp_Name"].ToString(),
                                EmpNumber = reader["Emp_No"].ToString(),
                                EmpGrade = reader["Emp_Grade"].ToString(),
                                EmpMobile = reader["Emp_Mobile"].ToString()
                                // Add other properties as needed
                            };
                        }
                    }
                }
            }

            return Json(employeeInfo);
        }


        public class EmployeeInfo
        {
            public string EmpName { get; set; }
            public string EmpNumber { get; set; }
            public string EmpGrade { get; set; }
            public string EmpMobile { get; set; }
            // Add other properties as needed
        }
    }
}

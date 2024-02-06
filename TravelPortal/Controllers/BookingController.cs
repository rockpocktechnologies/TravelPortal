using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TravelPortal.Models;
using System.Reflection;
using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Net.Mail;
using static TravelPortal.Controllers.NewRequestController;
using System.Net;
using TravelPortal.Classes;

namespace TravelPortal.Controllers
{

    public class BookingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public BookingController(IConfiguration configuration, 
            IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }


        public IActionResult Index(string requestToken)
        {
            Booking bookingDetails = GetBookingDetails(requestToken);
            return View(bookingDetails);
        }

        private Booking GetBookingDetails(string requestToken)
        {
            Booking model = new Booking();
            List<MultiCityData> multiCityData = new List<MultiCityData>();

            using (SqlConnection connection = 
                new SqlConnection(_configuration.GetConnectionString("ConnectionString")))

            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("trvl_SPGetBookingDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RequestToken", requestToken));
                 

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.Read())
                        {
                            model.strTravelType = reader["strTravelType"] == DBNull.Value ? "" : reader["strTravelType"].ToString();
                            model.strTravelRequestNumber = reader["strTravelRequestNumber"] == DBNull.Value ? "" : reader["strTravelRequestNumber"].ToString();
                            model.strTravelMode = reader["strTravelMode"] == DBNull.Value ? "" : reader["strTravelMode"].ToString();
                            model.strTicketBookedBy = reader["strTicketBookedBy"] == DBNull.Value ? "" : reader["strTicketBookedBy"].ToString();
                            model.strTravelDestination = reader["strTravelDestination"] == DBNull.Value ? "" : reader["strTravelDestination"].ToString();
                            model.strdivTravelType = reader["strdivTravelType"] == DBNull.Value ? "" : reader["strdivTravelType"].ToString();
                            model.strtravelFrom = reader["strtravelFrom"] == DBNull.Value ? "" : reader["strtravelFrom"].ToString();
                            model.strtravelTo = reader["strtravelTo"] == DBNull.Value ? "" : reader["strtravelTo"].ToString();
                            // model.strtravelDepartureDate = reader["strtravelDepartureDate"] == DBNull.Value ? "" : reader["strtravelDepartureDate"].ToString();

                            if (reader["strtravelDepartureDate"] != DBNull.Value)
                            {
                                DateTime departureDate;
                                if (DateTime.TryParse(reader["strtravelDepartureDate"].ToString(), out departureDate))
                                {
                                    model.strtravelDepartureDate = departureDate.ToString("dd/MM/yyyy");
                                    if (model.strtravelDepartureDate == "01-01-1900")
                                    {
                                        model.strtravelDepartureDate = "";
                                    }
                                }
                                else
                                {
                                    model.strtravelDepartureDate = ""; // Handle invalid date format
                                }
                            }
                            else
                            {
                                model.strtravelDepartureDate = "";


                            }

                            if (reader["strttravelReturnDate"] != DBNull.Value)
                            {
                                DateTime rtnDate;
                                if (DateTime.TryParse(reader["strttravelReturnDate"].ToString(), out rtnDate))
                                {
                                    model.strttravelReturnDate = rtnDate.ToString("dd/MM/yyyy");
                                    if (model.strttravelReturnDate == "01-01-1900")
                                    {
                                        model.strttravelReturnDate = "";
                                    }
                                }
                                else
                                {
                                    model.strttravelReturnDate = ""; // Handle invalid date format
                                }
                            }
                            else
                            {
                                model.strttravelReturnDate = "";


                            }
                           // model.strttravelReturnDate = reader["strttravelReturnDate"] == DBNull.Value ? "" : reader["strttravelReturnDate"].ToString();

                            model.stremployeeNo = reader["stremployeeNo"] == DBNull.Value ? "" : reader["stremployeeNo"].ToString();
                            model.stremployeeName = reader["stremployeeName"] == DBNull.Value ? "" : reader["stremployeeName"].ToString();
                            model.strgrade = reader["strgrade"] == DBNull.Value ? "" : reader["strgrade"].ToString();
                            model.strposition = reader["strposition"] == DBNull.Value ? "" : reader["strposition"].ToString();
                            model.strdepartment = reader["strdepartment"] == DBNull.Value ? "" : reader["strdepartment"].ToString();
                            model.strlocation = reader["strlocation"] == DBNull.Value ? "" : reader["strlocation"].ToString();
                            model.strmobileNumber = reader["strmobileNumber"] == DBNull.Value ? "" : reader["strmobileNumber"].ToString();
                            model.strage = reader["strage"] == DBNull.Value ? "" : reader["strage"].ToString();
                            model.strselectGender = reader["strselectGender"] == DBNull.Value ? "" : reader["strselectGender"].ToString();
                            model.strselectPurpose = reader["strselectPurpose"] == DBNull.Value ? "" : reader["strselectPurpose"].ToString();
                            model.strtravelDays = reader["strtravelDays"] == DBNull.Value ? "" : reader["strtravelDays"].ToString();
                            model.strmanager1 = reader["strmanager1"] == DBNull.Value ? "" : reader["strmanager1"].ToString();
                            model.strmanager2 = reader["strmanager2"] == DBNull.Value ? "" : reader["strmanager2"].ToString();
                            model.strdirector = reader["strdirector"] == DBNull.Value ? "" : reader["strdirector"].ToString();
                            model.stradvanceAmount = reader["stradvanceAmount"] == DBNull.Value ? "" : reader["stradvanceAmount"].ToString();
                            model.strselectCurrency = reader["strselectCurrency"] == DBNull.Value ? "" : reader["strselectCurrency"].ToString();
                            model.strselectProjectCode = reader["strselectProjectCode"] == DBNull.Value ? "" : reader["strselectProjectCode"].ToString();
                            model.strcomments = reader["strcomments"] == DBNull.Value ? "" : reader["strcomments"].ToString();
                            model.strRequestedFor = reader["strRequestedFor"] == DBNull.Value ? "" : reader["strRequestedFor"].ToString();
                            model.strEmpType = reader["strEmpType"] == DBNull.Value ? "" : reader["strEmpType"].ToString();
                            model.selectCurrency = reader["selectCurrency"] == DBNull.Value ? "" : reader["selectCurrency"].ToString();
                            model.strNameonPassport = reader["strNameonPassport"] == DBNull.Value ? "" : reader["strNameonPassport"].ToString();
                            model.strPassportNumber = reader["strPassportNumber"] == DBNull.Value ? "" : reader["strPassportNumber"].ToString();
                            //model.strPassportIssueDate = reader["strPassportIssueDate"] == DBNull.Value ? "" : reader["strPassportIssueDate"].ToString();
                            if (reader["strPassportIssueDate"] != DBNull.Value)
                            {
                                DateTime rtnDate;
                                if (DateTime.TryParse(reader["strPassportIssueDate"].ToString(), out rtnDate))
                                {
                                    model.strPassportIssueDate = rtnDate.ToString("dd/MM/yyyy");
                                    if (model.strPassportIssueDate == "01-01-1900")
                                    {
                                        model.strPassportIssueDate = "";
                                    }
                                }
                                else
                                {
                                    model.strPassportIssueDate = ""; // Handle invalid date format
                                }
                            }
                            else
                            {
                                model.strPassportIssueDate = "";


                            }
                           // model.strPassportExpiryDate = reader["strPassportExpiryDate"] == DBNull.Value ? "" : reader["strPassportExpiryDate"].ToString();
                            if (reader["strPassportExpiryDate"] != DBNull.Value)
                            {
                                DateTime rtnDate;
                                if (DateTime.TryParse(reader["strPassportExpiryDate"].ToString(), out rtnDate))
                                {
                                    model.strPassportExpiryDate = rtnDate.ToString("dd/MM/yyyy");
                                    if (model.strPassportExpiryDate == "01-01-1900")
                                    {
                                        model.strPassportExpiryDate = "";
                                    }
                                }
                                else
                                {
                                    model.strPassportExpiryDate = ""; // Handle invalid date format
                                }
                            }
                            else
                            {
                                model.strPassportIssueDate = "";


                            }


                            model.strNameofNominee = reader["strNameofNominee"] == DBNull.Value ? "" : reader["strNameofNominee"].ToString();
                            model.strRelationwithNominee = reader["strRelationwithNominee"] == DBNull.Value ? "" : reader["strRelationwithNominee"].ToString();
                            //model.strPassportIssueDate = reader["strPassportIssueDate"] == DBNull.Value ? "" : reader["strPassportIssueDate"].ToString();
                            if (reader["strDOBofNominee"] != DBNull.Value)
                            {
                                DateTime rtnDate;
                                if (DateTime.TryParse(reader["strDOBofNominee"].ToString(), out rtnDate))
                                {
                                    model.strDOBofNominee = rtnDate.ToString("dd/MM/yyyy");
                                    if (model.strDOBofNominee == "01-01-1900")
                                    {
                                        model.strDOBofNominee = "";
                                    }
                                }
                                else
                                {
                                    model.strDOBofNominee = ""; // Handle invalid date format
                                }
                            }
                            else
                            {
                                model.strDOBofNominee = "";


                            }

                            model.strGenderofNominee = reader["strGenderofNominee"] == DBNull.Value ? "" : reader["strGenderofNominee"].ToString();


                            model.stremployeeEmail = reader["stremployeeEmail"] == DBNull.Value ? "" : reader["stremployeeEmail"].ToString();
                            // model.IsCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"]);
                            model.CancelledCount = reader["CancelledCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CancelledCount"].ToString());

                            model.IsTripCancelled = reader["IsTripCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsTripCancelled"].ToString());

                            model.strPurpose = reader["strPurpose"] == DBNull.Value ? "" : reader["strPurpose"].ToString();
                           
                            model.strJobCode = reader["strJobCode"] == DBNull.Value ? "" : reader["strJobCode"].ToString();

                        }
                        reader.NextResult(); // Move to the next result set

                        while (reader.Read())
                        {
                            // Create a MultiCityTravelModel and populate it
                            MultiCityData city = new MultiCityData();

                            city.travelFrom = reader["SourceCity"] == DBNull.Value ? "" : reader["SourceCity"].ToString();
                            city.travelTo = reader["DestinationCity"] == DBNull.Value ? "" : reader["DestinationCity"].ToString();
                           // city.travelDepartureDate = reader["DepartureDate"] == DBNull.Value ? "" : reader["DepartureDate"].ToString();
                            if (reader["DepartureDate"] != DBNull.Value)
                            {
                                DateTime rtnDate;
                                if (DateTime.TryParse(reader["DepartureDate"].ToString(), out rtnDate))
                                {
                                    city.travelDepartureDate = rtnDate.ToString("dd/MM/yyyy");
                                    if (city.travelDepartureDate == "01-01-1900")
                                    {
                                        city.travelDepartureDate = "";
                                    }
                                }
                                else
                                {
                                    city.travelDepartureDate = ""; // Handle invalid date format
                                }
                            }
                            else
                            {
                                model.strttravelReturnDate = "";


                            }
                            multiCityData.Add(city);
                        }


                    }
                }
                model.MultiCityData = multiCityData;
            }

            return model;
        }

        [HttpPost]
        public IActionResult SaveBooking([FromBody] List<BookingSave> bookingData)
        {

            string TravelRequestToken = bookingData[0].RequestToken;
            bool IsSubmit = bookingData[0].IsSubmit;
            CommonFunctions cmn = new CommonFunctions(_configuration);

            List<int> insertedBookingIDs = new List<int>();
            using (SqlConnection connection =
              new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("trvl_SPInsertBookingDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var dataTable = new DataTable();
                    
                        dataTable.Columns.Add("Bookingid", typeof(int));
                    dataTable.Columns.Add("FromPlace", typeof(string));
                    dataTable.Columns.Add("ToPlace", typeof(string));
                    dataTable.Columns.Add("DepartureDate", typeof(string));
                    dataTable.Columns.Add("ArrivalDate", typeof(string));
                    dataTable.Columns.Add("TravelClass", typeof(string));
                    dataTable.Columns.Add("AirlineNumber", typeof(string));
                    dataTable.Columns.Add("PnrNumber", typeof(string));
                    dataTable.Columns.Add("InvoiceNumber", typeof(string));
                    dataTable.Columns.Add("InvoiceDate", typeof(string));
                    dataTable.Columns.Add("Fare", typeof(string));
                    dataTable.Columns.Add("Tax", typeof(string));
                    dataTable.Columns.Add("TotalAmount", typeof(string));
                    dataTable.Columns.Add("Discount", typeof(string));
                    dataTable.Columns.Add("ServiceCharges", typeof(string));
                    dataTable.Columns.Add("ServiceTax", typeof(string));
                    dataTable.Columns.Add("NetAmount", typeof(string));
                    dataTable.Columns.Add("Attachment", typeof(string));
                    dataTable.Columns.Add("TicketStatus", typeof(string));
                    dataTable.Columns.Add("CancellationCharges", typeof(string));
                    dataTable.Columns.Add("Comments", typeof(string));

                    foreach (var booking in bookingData)
                    {
                        dataTable.Rows.Add(
                            booking.Bookingid,
                            booking.FromPlace,
                            booking.ToPlace,
                            cmn.ConvertDateFormatddmmyytommddyyDuringSave(booking.DepartureDate),
                            cmn.ConvertDateFormatddmmyytommddyyDuringSave(booking.ArrivalDate),
                            booking.TravelClass,
                            booking.AirlineNumber,
                            booking.PnrNumber,
                            booking.InvoiceNumber,
                            cmn.ConvertDateFormatddmmyytommddyyDuringSave(booking.InvoiceDate),
                            booking.Fare,
                            booking.Tax,
                            booking.TotalAmount,
                            booking.Discount,
                            booking.ServiceCharges,
                            booking.ServiceTax,
                            booking.NetAmount,
                            booking.Attachment,
                            booking.TicketStatus,
                            booking.CancellationCharges,
                            booking.Comments
                        );
                    }

                    var parameter = new SqlParameter("@bookingDetails", SqlDbType.Structured)
                    {
                        TypeName = "BookingDetailsType",
                        Value = dataTable
                    };
                    cmd.Parameters.AddWithValue("@TravelRequestToken", TravelRequestToken);
                    cmd.Parameters.AddWithValue("@IsSubmit", IsSubmit);

                    cmd.Parameters.Add(parameter);

                    using (var reader = cmd.ExecuteReader())
                    {
                       

                        while (reader.Read())
                        {
                            int insertedID = reader.GetInt32(0);
                            insertedBookingIDs.Add(insertedID);
                        }

                        // The insertedBookingIDs list now contains the IDs of the inserted records
                    }

                }
            }

            

            return Json(insertedBookingIDs);
        }

        [HttpPost]
        public ActionResult SendEmailWithAttachments(string requestToken, 
            string employeeName, string employeeEmail)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (var cmd = new SqlCommand("trvl_SPGetAttachmentsForEmail", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters as needed
                    cmd.Parameters.Add(new SqlParameter("@requestToken", requestToken));

                    using (var reader = cmd.ExecuteReader())
                    {
                        List<Attachments> attachments = new List<Attachments>();

                        while (reader.Read())
                        {
                            Attachments att = new Attachments();
                            // Read attachment details from the result set
                            string originalFileName = reader["OriginalFileName"].ToString();
                            byte[] attachmentData = (byte[])reader["AttachmentData"];

                            // Create an Attachment object and add it to the list
                            //Attachment attachment = new Attachment(new MemoryStream(attachmentData), originalFileName);
                            att.AttachmentData = attachmentData;
                            att.OriginalFileName = originalFileName;

                            attachments.Add(att);
                        }

                        SendEmailController eml = new SendEmailController(_configuration);

                        eml.SendEmailToUserThatTicketBooked(attachments, requestToken,
                            employeeName, employeeEmail); ;
                    }
                }
            }
            return Json("1");
        }


        public IActionResult DownloadAttachment(int BookingId)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_SPGetBookingAttachment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookingId", BookingId);

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


        public async Task<IActionResult> UploadAttachments(string RequestToken,
            List<IFormFile> attachments, string strEmployeeName,
            string strEmployeeEmail,
            string bookingIds)
        {
            try
            {
                List<int> insertedBookingIDs = JsonConvert.DeserializeObject<List<int>>(bookingIds);

                var attachmentList = new List<Attachments>();

                int i = 0;
                foreach (var file in attachments)
                {
                    if (file != null && file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            var attachmentData = memoryStream.ToArray();

                            // Create an Attachment object
                            var attachment = new Attachments
                            {
                                BookingId = insertedBookingIDs[i],
                                OriginalFileName = file.FileName,
                                AttachmentData = attachmentData // Save the file contents as bytes
                            };

                            attachmentList.Add(attachment);
                        }
                    }
                    i = i + 1;
                }



                // Save attachment records to the database
                using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();

                    using (var cmd = new SqlCommand("trvl_SPInsertBookingAttachments", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        var dtBookingAttachments = new DataTable();
                        dtBookingAttachments.Columns.Add("BookingId", typeof(int));
                        dtBookingAttachments.Columns.Add("OriginalFileName", typeof(string));
                        dtBookingAttachments.Columns.Add("AttachmentData", typeof(byte[]));

                        foreach (var attachment in attachmentList)
                        {
                            dtBookingAttachments.Rows.Add(attachment.BookingId, 
                                attachment.OriginalFileName, attachment.AttachmentData);
                        }

                        // Define a parameter for the DataTable
                        var parameter = new SqlParameter("@dtBookingAttachments", SqlDbType.Structured)
                        {
                            TypeName = "dtBookingAttachmentType", // Replace with your actual table type name
                            Value = dtBookingAttachments
                        };
                        cmd.Parameters.AddWithValue("@TravelRequestToken", RequestToken);
                        cmd.Parameters.Add(parameter);

                        cmd.ExecuteNonQuery();
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
                // Handle any exceptions
                return Json("0");
            }
        }


        [HttpPost]
        public JsonResult GetEditDetails(string stringTravelRequestToken)
        {
            var editDetails = GetEditBookingDetailsFromDatabase(stringTravelRequestToken);

            // Convert DataTable to List of Dictionary<string, object>
            var serializedData = DataTableToDictionaryList(editDetails);

            return Json(serializedData);
        }

        private List<Dictionary<string, object>> DataTableToDictionaryList(DataTable table)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                rows.Add(dict);
            }
            return rows;
        }

        private DataTable GetEditBookingDetailsFromDatabase(string stringTravelRequestToken)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("trvl_SPGetEditBookingDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters as needed
                    cmd.Parameters.Add(new SqlParameter("@TravelRequestToken", stringTravelRequestToken));

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        FormatDateColumns(dt);
                        return dt;
                    }
                }
            }
        }

        private void FormatDateColumns(DataTable dataTable)
        {
            CommonFunctions cmn = new CommonFunctions(_configuration);
            // Assuming "DepartureDate" and "InvoiceDate" are the date columns in your DataTable
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["DepartureDate"] != DBNull.Value)
                {
                    row["DepartureDate"] = cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(row["DepartureDate"].ToString());
                }

                if (row["InvoiceDate"] != DBNull.Value)
                {
                    row["InvoiceDate"] = cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(row["InvoiceDate"].ToString());
                }
                // Add more columns as needed
            }
        }

      

        //[HttpPost]
        //public async Task<IActionResult> UploadAttachments(string RequestToken, 
        //    List<IFormFile> attachments)
        //{
        //    try
        //    {
        //        var attachmentList = new List<Attachments>();

        //        // Loop through the uploaded files
        //        foreach (var file in attachments)
        //        {
        //            if (file != null && file.Length > 0)
        //            {
        //                var guidFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "attachments", guidFileName);

        //                // Save the file to the "attachments" folder
        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }

        //                // Create an Attachment object
        //                var attachment = new Attachments
        //                {
        //                    BookingId = 0,
        //                    OriginalFileName = file.FileName,
        //                    FileNameOnDisk = guidFileName
        //                };

        //                attachmentList.Add(attachment);
        //            }
        //        }

        //        var attachmentPaths = new List<string>();

        //        foreach (var attachment in attachmentList)
        //        {
        //            var attachmentPath = Path.Combine(_hostingEnvironment.WebRootPath, "attachments", attachment.FileNameOnDisk);
        //            attachmentPaths.Add(attachmentPath);
        //        }

        //        SendEmailController eml = new SendEmailController(_configuration);

        //        eml.SendEmailToUserThatEmailBooked(attachmentPaths, RequestToken);

        //        var dtBookingAttachments = new DataTable();
        //        dtBookingAttachments.Columns.Add("BookingId", typeof(int));
        //        dtBookingAttachments.Columns.Add("OriginalFileName", typeof(string));
        //        dtBookingAttachments.Columns.Add("FileNameOnDisk", typeof(string));
        //        foreach (var attachment in attachmentList)
        //        {
        //            dtBookingAttachments.Rows.Add(attachment.BookingId, attachment.OriginalFileName, attachment.FileNameOnDisk);
        //        }


        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
        //        {
        //            connection.Open();

        //            using (var cmd = new SqlCommand("trvl_SPInsertBookingAttachments", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                // Define a parameter for the DataTable
        //                var parameter = new SqlParameter("@dtBookingAttachments", SqlDbType.Structured)
        //                {
        //                    TypeName = "dtBookingAttachmentType", // Replace with your actual table type name
        //                    Value = dtBookingAttachments
        //                };
        //                cmd.Parameters.AddWithValue("@TravelRequestToken", RequestToken);
        //                cmd.Parameters.Add(parameter);

        //                cmd.ExecuteNonQuery();
        //            }
        //        }


        //        // Save attachment records to the database
        //        //foreach (var attachment in attachmentList)
        //        //{
        //        //    // Insert the attachment into the "trvl_tblbookingattachments" table
        //        //    var query = "INSERT INTO trvl_tblbookingattachments (BookingId, OriginalFileName, FileNameOnDisk) VALUES (@BookingId, @OriginalFileName, @FileNameOnDisk)";
        //        //    var parameters = new
        //        //    {
        //        //        BookingId = attachment.BookingId,
        //        //        OriginalFileName = attachment.OriginalFileName,
        //        //        FileNameOnDisk = attachment.FileNameOnDisk
        //        //    };
        //        //    await _dbConnection.ExecuteAsync(query, parameters);
        //        //}

        //        return Ok("Attachments uploaded and saved successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions
        //        return BadRequest("An error occurred while uploading and saving attachments.");
        //    }
        //}



        //    [HttpPost]

        //    public async Task<IActionResult> Create(
        //        int? TravelRequestId,
        //        string FromPlace,
        //        string ToPlace,
        //        DateTime? DepartureDate,
        //        DateTime? ArrivalDate,
        //        string TravelClass,
        //        string AirlineNumber,
        //        string PnrNumber,
        //        string InvoiceNumber,
        //        DateTime? InvoiceDate,
        //        decimal? Fare,
        //        decimal? Tax,
        //        decimal? TotalAmount,
        //        decimal? Discount,
        //        decimal? ServiceCharges,
        //        decimal? ServiceTax,
        //        decimal? NetAmount,
        //        string Attachment,
        //        string TicketStatus,
        //        decimal? CancellationCharges,
        //        string Comments)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                var connectionString = _configuration.GetConnectionString("DefaultConnection");
        //                using (var connection = new SqlConnection(connectionString))
        //                {
        //                    connection.Open();
        //                    using (var command = new SqlCommand("InsertBooking", connection))
        //                    {
        //                        command.CommandType = CommandType.StoredProcedure;
        //                        command.Parameters.AddWithValue("@TravelRequestId", TravelRequestId);
        //                        command.Parameters.AddWithValue("@FromPlace", FromPlace);
        //                        command.Parameters.AddWithValue("@ToPlace", ToPlace);
        //                        command.Parameters.AddWithValue("@DepartureDate", DepartureDate);
        //                        command.Parameters.AddWithValue("@ArrivalDate", ArrivalDate);
        //                        command.Parameters.AddWithValue("@TravelClass", TravelClass);
        //                        command.Parameters.AddWithValue("@AirlineNumber", AirlineNumber);
        //                        command.Parameters.AddWithValue("@PnrNumber", PnrNumber);
        //                        command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);
        //                        command.Parameters.AddWithValue("@InvoiceDate", InvoiceDate);
        //                        command.Parameters.AddWithValue("@Fare", Fare);
        //                        command.Parameters.AddWithValue("@Tax", Tax);
        //                        command.Parameters.AddWithValue("@TotalAmount", TotalAmount);
        //                        command.Parameters.AddWithValue("@Discount", Discount);
        //                        command.Parameters.AddWithValue("@ServiceCharges", ServiceCharges);
        //                        command.Parameters.AddWithValue("@ServiceTax", ServiceTax);
        //                        command.Parameters.AddWithValue("@NetAmount", NetAmount);
        //                        command.Parameters.AddWithValue("@Attachment", Attachment);
        //                        command.Parameters.AddWithValue("@TicketStatus", TicketStatus);
        //                        command.Parameters.AddWithValue("@CancellationCharges", CancellationCharges);
        //                        command.Parameters.AddWithValue("@Comments", Comments);

        //                        await command.ExecuteNonQueryAsync();
        //                    }
        //                }
        //                return RedirectToAction("Index");
        //            }
        //            catch (SqlException ex)
        //            {
        //                ModelState.AddModelError(string.Empty, "Unable to save changes.");
        //            }
        //        }
        //        return View();
        //    }
    }


}
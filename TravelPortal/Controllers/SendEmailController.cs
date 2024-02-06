using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mime;
using TravelPortal.Models;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Net.Http;
using TravelPortal.Classes;
using Microsoft.Graph.Beta.Models.ManagedTenants;

namespace TravelPortal.Controllers
{
    public class SendEmailController
    {
        private readonly IConfiguration _configuration;

        public SendEmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendApprovalEmail(int newTravelRequestId, string managerName, 
            string managerEmail, string tokenGuid,
            string approvalUrl, string rejectUrl)
        {

            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            string fromEmail = _configuration["EmailSettings:FromEmail"];

            string emailTemplate = File.ReadAllText("./EmailTemplates/ApprovalEmailToManager.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(managerName, newTravelRequestId,
                emailTemplate,"");


            emailTemplate = emailTemplate.Replace("{APPROVAL_LINK}", $"{approvalUrl}?token={tokenGuid}");
            emailTemplate = emailTemplate.Replace("{REJECT_LINK}", $"{rejectUrl}?token={tokenGuid}");

            SendMail(managerEmail, "Travel Request Approval", emailTemplate,null,
                newTravelRequestId,"");
        

        }

        public void SendNewRequestEmail(int newTravelRequestId, string empname,
            string empemail)
        {

            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            string fromEmail = _configuration["EmailSettings:FromEmail"];

            string emailTemplate = File.ReadAllText("./EmailTemplates/NewRequestEmail.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(empname, newTravelRequestId,
                emailTemplate, "");


            SendMail(empemail, "New Travel Request", emailTemplate,
                null, newTravelRequestId,"");


        }

        public void RejectRequest(string reason, string RequestOwnerName,
                               string  RequestOwnerEmail, string subject,
                               int TravelRequestId, 
                               string strRequType)
        {


            string emailTemplate = File.ReadAllText("./EmailTemplates/RejectRequest.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(RequestOwnerName,
                TravelRequestId,
                emailTemplate, "");

            emailTemplate = emailTemplate.Replace("{RequestType}", strRequType);
            emailTemplate = emailTemplate.Replace("{Reason}", reason);  
            SendMail(RequestOwnerEmail, subject, emailTemplate, null,
                TravelRequestId,"");

            

        }

        public void SendExpenseApprovalEmail(int newTravelRequestId, string managerName,
         string managerEmail, string tokenGuid,
         string approvalUrl, string rejectUrl, string detailsHtml, string SName,
           string trvlReqNumber )
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/SendEmailForExpenseApprovalToManager.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);
            emailTemplate = trvl.FetchTravelRequestDetailsData(managerName, newTravelRequestId,
       emailTemplate, "");

            emailTemplate = emailTemplate.Replace("{ExpenseDetails}", detailsHtml);
            
            emailTemplate = emailTemplate.Replace("{ReceiverName}", managerName);
            emailTemplate = emailTemplate.Replace("{SenderName}", SName);

            emailTemplate = emailTemplate.Replace("{TravelRequestNumber}", trvlReqNumber);
            
            emailTemplate = emailTemplate.Replace("{APPROVAL_LINK}", $"{approvalUrl}?token={tokenGuid}");
            emailTemplate = emailTemplate.Replace("{REJECT_LINK}", $"{rejectUrl}?token={tokenGuid}");

            SendMail(managerEmail, "Expense Approval", emailTemplate,
                null, newTravelRequestId, "");

        }

        public void SendEmailTAccounforAdvanceAmount(int TravelRequestId,string accountsEmail,
            string accountsName)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/AdvanceAmountEmailToAccountsTeam.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(accountsName, TravelRequestId,
                emailTemplate,"");

            SendMail(accountsEmail, "Request for advance amount", emailTemplate,
                null, TravelRequestId,"");

        }

        public void SendEmailToAdmin(int TravelRequestId, string AdminEmail,
     string AdminName)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/EmailToAdmin.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(AdminName, TravelRequestId,
                emailTemplate, "");

            SendMail(AdminEmail, "New Travel Request", emailTemplate, null, TravelRequestId,"");

        }

        public void SendEmailToInsurance(int TravelRequestId, string InsurancePersonEmail,
  string InsurancePersonName)
        {
            string emailTemplate = File.ReadAllText("./EmailTemplates/InsuranceEmail.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(InsurancePersonName, TravelRequestId,
                emailTemplate, "");

            SendMail(InsurancePersonEmail, "New Travel Request", emailTemplate, null, TravelRequestId);


        }

        public void SendEmailToTravelDeskForBooking(int TravelRequestId, string travelDeskEmail,
           string travelDeskName)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/TravelDeskTicketBooking.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(travelDeskName, TravelRequestId,
                emailTemplate, "");


            SendMail(travelDeskEmail, "Request for ticket booking", emailTemplate, null, TravelRequestId);

        }

        
      public void SendEmailToTravelDeskForCancelTicket(int TravelRequestId, string travelDeskEmail,
           string travelDeskName)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/TicketCancelMailToTravelDesk.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(travelDeskName, TravelRequestId,
                emailTemplate, "");


            SendMail(travelDeskEmail, "Request for ticket cancellation", emailTemplate, null, TravelRequestId);

        }
        public void SendEmailToUserThatTicketBooked(List<Attachments> attchments, string token,
          string strEmployeeName, string strEmployeeEmail)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/BookingSuccess.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(strEmployeeName, 0,
                emailTemplate, token);

            SendMail(strEmployeeEmail, "Travel Request Booked", emailTemplate, attchments,
                0, token);

        }


        public void SendEmailToAccounForExpenseApproval(int TravelRequestId,
            string accountsEmail, string accountsName, string detailsHtml,
            string approvalUrl, string rejectUrl,string tokenGuid)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/SendEmailForExpenseApprovalToManager.html");

              emailTemplate = emailTemplate.Replace("{APPROVAL_LINK}", $"{approvalUrl}?token={tokenGuid}");
            emailTemplate = emailTemplate.Replace("{REJECT_LINK}", $"{rejectUrl}?token={tokenGuid}");


            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(accountsName, TravelRequestId,
                emailTemplate, "");

            if(detailsHtml != "")
            {
                emailTemplate = emailTemplate.Replace("{ExpenseDetails}", detailsHtml);

            }
            else
            {
                emailTemplate = emailTemplate.Replace("{ExpenseDetails}", "");

            }
            SendMail(accountsEmail, "Request for expense approval", emailTemplate, null, TravelRequestId,"");

        }

        public void SendEmailToUserThatExpenseApproved(int TravelRequestId,
   string strEmployeeName, string strEmployeeEmail, string approver)
        {

            string emailTemplate = File.ReadAllText("./EmailTemplates/ExpenseApproved.html");

            FetchTravelRequestDetails trvl = new FetchTravelRequestDetails(_configuration);

            emailTemplate = trvl.FetchTravelRequestDetailsData(strEmployeeName, 
                TravelRequestId,
                emailTemplate, "");

            emailTemplate = emailTemplate.Replace("{Approver}", approver);


            SendMail(strEmployeeEmail, "Expense Approved by " + approver,
                emailTemplate,null, TravelRequestId, "");
        }


        public void SendMail(string toEmail, string subject, 
            string body, List<Attachments>? attchments = null,
            int travelRequestId = 0, string RequesToken = "")
        {

            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            var From = _configuration["EmailSettings:FromEmail"];
            string isLocal = _configuration["EmailSettings:IsLocal"];

            if (isLocal == "true")
            {
                string port = _configuration["EmailSettings:Port"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                using (var smtpClient = new SmtpClient(smtpServer))
                {

                    using (var mailMessage = new MailMessage())
                    {

                        smtpClient.Port = Convert.ToInt32(port); // Use port 587 for secure SMTP
                        smtpClient.EnableSsl = true; // Enable SSL for secure communication

                        mailMessage.From = new MailAddress(From);
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = subject;
                        mailMessage.IsBodyHtml = true;

                        // Read the email template content from the file

                        mailMessage.Body = body;

                        if(attchments != null) {
                            foreach (Attachments attchment in attchments)
                            {
                                Attachment att = new Attachment(new MemoryStream(attchment.AttachmentData),
                                    attchment.OriginalFileName);

                                mailMessage.Attachments.Add(att);
                            }
                        }

                        

                        smtpClient.Credentials =
                            new NetworkCredential(username, password); // Use your Office 365 email and password


                        try
                        {
                            smtpClient.Send(mailMessage);

                            string bodyConetnt = "";
                            if(travelRequestId == 0)
                            {
                                bodyConetnt = RequesToken;
                            }
                            else
                            {
                                bodyConetnt = travelRequestId.ToString();
                            }

                            LogEmail(toEmail, subject, bodyConetnt, DateTime.Now);
                        }
                        catch (Exception ex)
                        {
                            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            string exceptionMessage = $"Exception in method '{methodName}'";

                            // Log or rethrow the exception with the updated message
                            var errorLogger = new ErrorLogger(_configuration);
                            errorLogger.LogError(ex, exceptionMessage);
                        }
                    }
                }
            }
            else
            {
                using (var smtpClient = new SmtpClient(smtpServer))
                {

                    using (var mailMessage = new MailMessage())
                    {
                        // Set the sender's email address
                        mailMessage.From = new MailAddress(From);

                        // Set the recipient's email address
                        mailMessage.To.Add(toEmail);

                        // Set the email subject and body
                        mailMessage.Subject = subject;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = body;

                        if (attchments != null)
                        {
                            foreach (Attachments attchment in attchments)
                            {
                                Attachment att = new Attachment(new MemoryStream(attchment.AttachmentData),
                                    attchment.OriginalFileName);

                                mailMessage.Attachments.Add(att);
                            }
                        }

                        try
                        {
                            smtpClient.Send(mailMessage);
                            LogEmail(toEmail,subject,body, DateTime.Now);
                        }
                        catch (Exception ex)
                        {
                            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            string exceptionMessage = $"Exception in method '{methodName}'";

                            // Log or rethrow the exception with the updated message
                            var errorLogger = new ErrorLogger(_configuration);
                            errorLogger.LogError(ex, exceptionMessage);
                        }
                    }
                }
             }
          }


        private void LogEmail(string toEmail, string subject, string body, DateTime sentDate)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {

                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("trvl_sp_InsertEmailLog", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ToEmail", toEmail);
                        command.Parameters.AddWithValue("@Subject", subject);
                        command.Parameters.AddWithValue("@Body", body);
                        command.Parameters.AddWithValue("@SentDate", sentDate);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    string exceptionMessage = $"Exception in method '{methodName}'";

                    // Log or rethrow the exception with the updated message
                    var errorLogger = new ErrorLogger(_configuration);
                    errorLogger.LogError(ex, exceptionMessage);
                }
                
            }
        }
    }
}

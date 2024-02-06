using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TravelPortal.Models;

namespace TravelPortal.Controllers
{
    public class ApprovalLogController : Controller
    {
        private readonly string _connectionString;

        public ApprovalLogController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IActionResult Index(int? recordCount)
        {
            // Set a default value if recordCount is null
            recordCount ??= 200;

            // Call the stored procedure to get approval logs
            List<ApprovalLogModel> approvalLogs = GetApprovalLogs(recordCount.Value);

            return View(approvalLogs);
        }

        private List<ApprovalLogModel> GetApprovalLogs(int recordCount)
        {
            List<ApprovalLogModel> approvalLogs = new List<ApprovalLogModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("trvl_sp_GetApprovalLogs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RecordCount", recordCount);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ApprovalLogModel log = new ApprovalLogModel
                            {
                                ApprovalLogId = reader.GetInt32(reader.GetOrdinal("ApprovalLogId")),
                                TravelRequestId = reader.GetInt32(reader.GetOrdinal("TravelRequestId")),
                                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                                NameOfReceiver = reader.GetString(reader.GetOrdinal("NameOfReceiver")),
                                IsSentMailToTravelDesk = reader.GetBoolean(reader.GetOrdinal("IsSentMailToTravelDesk")),
                                IsAdvanceAmountMailToAccounts = reader.GetBoolean(reader.GetOrdinal("IsAdvanceAmountMailToAccounts")),
                                IsSentMailToAdmin = reader.GetBoolean(reader.GetOrdinal("IsSentMailToAdmin")),
                                IsSentMailToInsurance = reader.GetBoolean(reader.GetOrdinal("IsSentMailToInsurance")),
                                RequestOwnerName = reader.GetString(reader.GetOrdinal("RequestOwnerName")),
                                RequestOwnerEmail = reader.GetString(reader.GetOrdinal("RequestOwnerEmail")),
                                RequestNumber = reader.GetString(reader.GetOrdinal("RequestNumber")),
                                IsMakeStatusBookedWhenbySelf = reader.GetBoolean(reader.GetOrdinal("IsMakeStatusBookedWhenbySelf")),
                                LogToken = reader.GetString(reader.GetOrdinal("LogToken")),
                                LogDate = reader.GetDateTime(reader.GetOrdinal("LogDate"))
                            };

                            approvalLogs.Add(log);
                        }
                    }
                }
            }

            return approvalLogs;
        }
    }
}

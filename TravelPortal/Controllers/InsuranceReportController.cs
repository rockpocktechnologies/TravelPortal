using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data.SqlClient;
using TravelPortal.Models;

namespace TravelPortal.Controllers
{
    public class InsuranceReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly IConfiguration _configuration;

        public InsuranceReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<ReportModel> reportData = GetReportDataFromDatabase();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                
                worksheet.Cells["A1"].LoadFromCollection(new List<string>
                {
                    // "TravelRequestNumber", "Departure_City", "Destination_City", "Emp_No", "Emp_Name","Emp_Age", "Emp_Gender", "Emp_Grade", "Departure_Date", "Return_Date","Travel_Days", "Emp_Passport_Name", "Emp_Passport_Int", "Emp_Passport_Issue_Date","Emp_Passport_Expiry_Date", "NameofNominee", "RelationwithNominee", "DOBofNominee"
                    // Add other headers
                    // Include all other properties here
                }, true);


                for (int i = 0; i < reportData.Count; i++)
                {

                    worksheet.Cells[i + 2, 1].Value = reportData[i].TravelRequestNumber;
                    worksheet.Cells[i + 2, 2].Value = reportData[i].Departure_City;
                    worksheet.Cells[i + 2, 3].Value = reportData[i].Destination_City;
                    worksheet.Cells[i + 2, 4].Value = reportData[i].Emp_No;
                    worksheet.Cells[i + 2, 5].Value = reportData[i].Emp_Name;
                    worksheet.Cells[i + 2, 6].Value = reportData[i].Emp_Age;
                    worksheet.Cells[i + 2, 7].Value = reportData[i].Emp_Gender;
                    worksheet.Cells[i + 2, 8].Value = reportData[i].Emp_Grade;
                    worksheet.Cells[i + 2, 9].Value = reportData[i].Departure_Date;
                    worksheet.Cells[i + 2, 9].Style.Numberformat.Format = "dd-mm-yyyy";

                    worksheet.Cells[i + 2, 10].Value = reportData[i].Return_Date;
                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "dd-mm-yyyy";

                    worksheet.Cells[i + 2, 11].Value = reportData[i].Travel_Days;
                    worksheet.Cells[i + 2, 12].Value = reportData[i].Emp_Passport_Name;
                    worksheet.Cells[i + 2, 13].Value = reportData[i].Emp_Passport_Int;

                    worksheet.Cells[i + 2, 14].Value = reportData[i].Emp_Passport_Issue_Date;
                    worksheet.Cells[i + 2, 14].Style.Numberformat.Format = "dd-mm-yyyy";

                    worksheet.Cells[i + 2, 15].Value = reportData[i].Emp_Passport_Expiry_Date;
                    worksheet.Cells[i + 2, 15].Style.Numberformat.Format = "dd-mm-yyyy";

                    worksheet.Cells[i + 2, 16].Value = reportData[i].NameofNominee;
                    worksheet.Cells[i + 2, 17].Value = reportData[i].RelationwithNominee;

                    worksheet.Cells[i + 2, 18].Value = reportData[i].DOBofNominee;
                    worksheet.Cells[i + 2, 18].Style.Numberformat.Format = "dd-mm-yyyy";

                    
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Cells["A1"].Value = "TravelRequestNumber";
                worksheet.Cells["B1"].Value = "Departure_City";
                worksheet.Cells["D1"].Value = "Destination_City";
                worksheet.Cells["E1"].Value = "Emp_No";
                worksheet.Cells["F1"].Value = "Emp_Name";
                worksheet.Cells["G1"].Value = "Emp_Age";
                worksheet.Cells["H1"].Value = "Emp_Gender";
                worksheet.Cells["I1"].Value = "Departure_Date";
                worksheet.Cells["J1"].Value = "Return_Date";
                worksheet.Cells["K1"].Value = "Travel_Days";
                worksheet.Cells["L1"].Value = "Emp_Passport_Name";
                worksheet.Cells["M1"].Value = "Emp_Passport_Int";
                worksheet.Cells["N1"].Value = "Emp_Passport_Issue_Date";
                worksheet.Cells["O1"].Value = "Emp_Passport_Expiry_Date";
                worksheet.Cells["P1"].Value = "NameofNominee";
                worksheet.Cells["Q1"].Value = "RelationwithNominee";
                worksheet.Cells["R1"].Value = "DOBofNominee";

                var stream = new MemoryStream();
                package.SaveAs(stream);

                var fileName = $"Insurance Report_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx";
                var departureDateColumn = worksheet.Cells["H:H"];
                departureDateColumn.Style.Numberformat.Format = "dd-mm-yyyy";


                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        private List<ReportModel> GetReportDataFromDatabase()
        {
            return GetReportDataFromDatabase(null, null);
        }

        private List<ReportModel> GetReportDataFromDatabase(DateTime? fromDate, DateTime? toDate)
        {
            List<ReportModel> reportData = new List<ReportModel>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                using (var command = new SqlCommand("trvl_GetInsuranceReportData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FromDate", fromDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ToDate", toDate ?? (object)DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReportModel model = new ReportModel
                            {

                                TravelRequestNumber = reader["TravelRequestNumber"] == DBNull.Value ? "" : reader["TravelRequestNumber"].ToString(),
                                Departure_City = reader["Departure_City"] == DBNull.Value ? "" : reader["Departure_City"].ToString(),
                                Destination_City = reader["Destination_City"] == DBNull.Value ? "" : reader["Destination_City"].ToString(),
                                Emp_No = reader["Emp_No"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Emp_No"]),
                                Emp_Name = reader["Emp_Name"] == DBNull.Value ? "" : reader["Emp_Name"].ToString(),
                                Emp_Age = reader["Emp_Age"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Emp_Age"]),
                                Emp_Gender = reader["Emp_Gender"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Emp_Gender"]),
                                Emp_Grade = reader["Emp_Grade"] == DBNull.Value ? "" : reader["Emp_Grade"].ToString(),
                                Departure_Date = reader["Departure_Date"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["Departure_Date"]),
                                Return_Date = reader["Return_Date"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["Return_Date"]),
                                Travel_Days = reader["Travel_Days"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Travel_Days"]),
                                Emp_Passport_Name = reader["Emp_Passport_Name"] == DBNull.Value ? "" : reader["Emp_Passport_Name"].ToString(),
                                Emp_Passport_Int = reader["Emp_Passport_Int"] == DBNull.Value ? "" : reader["Emp_Passport_Int"].ToString(),
                                Emp_Passport_Issue_Date = reader["Emp_Passport_Issue_Date"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["Emp_Passport_Issue_Date"]),
                                Emp_Passport_Expiry_Date = reader["Emp_Passport_Expiry_Date"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["Emp_Passport_Expiry_Date"]),
                                NameofNominee = reader["NameofNominee"] == DBNull.Value ? "" : reader["NameofNominee"].ToString(),
                                RelationwithNominee = reader["RelationwithNominee"] == DBNull.Value ? "" : reader["RelationwithNominee"].ToString(),
                                DOBofNominee = reader["DOBofNominee"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["DOBofNominee"]),

                            };

                            reportData.Add(model);
                        }
                    }
                }
            }

            return reportData;


        }

        //public IActionResult GetFilteredData(DateTime? fromDate, DateTime? toDate)
        //{
        //    List<ReportModel> reportData = GetReportDataFromDatabase(fromDate, toDate);
        //    return Json(reportData);
        //}

        //private List<ReportModel> GetReportDataFromDatabase(DateTime? fromDate, DateTime? toDate)
        //{
        //    // ... Your existing code ...

        //    using (var command = new SqlCommand("rockpock.trvl_GetInsuranceReportData", ))
        //    {
        //        command.CommandType = CommandType.StoredProcedure;

        //        // Add parameters for fromDate and toDate
        //        command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.Date)).Value = fromDate ?? DBNull.Value;
        //        command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.Date)).Value = toDate ?? DBNull.Value;

        //        // ... Your existing code ...
        //    }

        //    return reportData;
        //}
        //private DateTime GetDateTime(SqlDataReader reader, string columnName)
        //{
        //    int columnIndex = reader.GetOrdinal(columnName);

        //    if (!reader.IsDBNull(columnIndex))
        //    {
        //        object value = reader.GetValue(columnIndex);

        //        if (value is DateTime)
        //        {
        //            return (DateTime)value;
        //        }
        //        else if (value is string && DateTime.TryParse((string)value, out DateTime date))
        //        {
        //            return date;
        //        }
        //    }

        //    return default(DateTime); // or throw an exception based on your handling
        //}
        private static int GetInt32(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? 0 : reader.GetInt32(reader.GetOrdinal(columnName));
        }

        private static string GetString(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? string.Empty : reader.GetString(reader.GetOrdinal(columnName));
        }

        private static DateTime GetDateTime(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal(columnName));
        }
    }
    
}


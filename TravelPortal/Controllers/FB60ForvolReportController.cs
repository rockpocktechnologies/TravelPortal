using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data.SqlClient;
using TravelPortal.Models;

namespace TravelPortal.Controllers
{
    public class FB60ForvolReportController : Controller
    {
        private readonly IConfiguration _configuration;
        public FB60ForvolReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<FB60ForvolModel> reportData = GetForvolReportDataFromDatabase();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add first header row
                worksheet.Cells["A1"].LoadFromCollection(new List<string>
                {
                    //"TravelRequestId", "SeriesNo", "CurruntDate", "InvoiceDate", "KR", "InvoiceNumber", "OL", "Total"
                    // Add other headers
                    // Include all other properties here
                }, true);

                for (int i = 0; i < reportData.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = reportData[i].TravelRequestId;
                    worksheet.Cells[i + 3, 2].Value = "Null";
                    worksheet.Cells[i + 3, 3].Value = "Null";
                    //worksheet.Cells[i + 3, 3].Style.Numberformat.Format = "dd-mm-yyyy";
                    worksheet.Cells[i + 3, 5].Value = "Null";
                    worksheet.Cells[i + 3, 6].Value = "Null";
                    worksheet.Cells[i + 3, 8].Value = "Null";
                    worksheet.Cells[i + 3, 9].Value = "OL";
                    worksheet.Cells[i + 3, 10].Value = "INR";
                    worksheet.Cells[i + 3, 11].Value = "Null";
                    worksheet.Cells[i + 3, 12].Value = "Null";
                    worksheet.Cells[i + 3, 16].Value = "Null";
                    worksheet.Cells[i + 3, 17].Value = "Null";
                    worksheet.Cells[i + 3, 18].Value = "Null";
                    worksheet.Cells[i + 3, 20].Value = "Null";
                    worksheet.Cells[i + 3, 21].Value = "Null";
                    worksheet.Cells[i + 3, 22].Value = "Null";
                    worksheet.Cells[i + 3, 23].Value = "Null";
                    worksheet.Cells[i + 3, 24].Value = "Null";
                    worksheet.Cells[i + 3, 26].Value = "Null";
                    worksheet.Cells[i + 3, 27].Value = "Null";
                    worksheet.Cells[i + 3, 28].Value = "N";
                    worksheet.Cells[i + 3, 25].Value = reportData[i].Emp_Name;

                    worksheet.Cells[i + 3, 14].Value = reportData[i].Travel_Destination;

                    worksheet.Cells[i + 3, 4].Value = reportData[i].InvoiceDate;
                    worksheet.Cells[i + 3, 4].Style.Numberformat.Format = "dd-mm-yyyy";

                    // worksheet.Cells[i + 3, 5].Value = reportData[i].KR;
                    worksheet.Cells[i + 3, 7].Value = reportData[i].InvoiceNumber;
                    // worksheet.Cells[i + 3, 7].Value = reportData[i].OL;
                    worksheet.Cells[i + 3, 19].Value = reportData[i].TotalAmount;


                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    worksheet.Cells["A1"].Value = "Document No";
                    worksheet.Cells["A2"].Value = "BELNR";

                    worksheet.Cells["B1"].Value = "Item No";
                    worksheet.Cells["B2"].Value = "BUZEI";

                    worksheet.Cells["C1"].Value = "Posting date - YYYYMMDD";
                    worksheet.Cells["C2"].Value = "BUDAT";

                    worksheet.Cells["D1"].Value = "Document Date - YYYYMMDD";
                    worksheet.Cells["D2"].Value = "BLDAT";

                    worksheet.Cells["E1"].Value = "Document type";
                    worksheet.Cells["E2"].Value = "BLART";

                    worksheet.Cells["F1"].Value = "Company code";
                    worksheet.Cells["F2"].Value = "BUKRS";

                    worksheet.Cells["G1"].Value = "Reference";
                    worksheet.Cells["G2"].Value = "XBLNR";

                    worksheet.Cells["H1"].Value = "Header Text";
                    worksheet.Cells["H2"].Value = "BKTXT";

                    worksheet.Cells["I1"].Value = "Ledger";
                    worksheet.Cells["I2"].Value = "LDGRP";

                    worksheet.Cells["J1"].Value = "Currency";
                    worksheet.Cells["J2"].Value = "WAERS";

                    worksheet.Cells["K1"].Value = "Posting key";
                    worksheet.Cells["K2"].Value = "BSCHL";

                    worksheet.Cells["L1"].Value = "Account no.";
                    worksheet.Cells["L2"].Value = "HKONT";

                    worksheet.Cells["M1"].Value = "Item text";
                    worksheet.Cells["M2"].Value = "HOB";

                    worksheet.Cells["N1"].Value = "Business Place";
                    worksheet.Cells["N2"].Value = "BUPLA";

                    worksheet.Cells["O1"].Value = "Section Code";
                    worksheet.Cells["O2"].Value = "SECCO";

                    worksheet.Cells["P1"].Value = "Profit centre";
                    worksheet.Cells["P2"].Value = "PRCTR";

                    worksheet.Cells["Q1"].Value = "Cost centre";
                    worksheet.Cells["Q2"].Value = "KOSTL";

                    worksheet.Cells["R1"].Value = "WBS";
                    worksheet.Cells["R2"].Value = "PS_POSID";

                    worksheet.Cells["S1"].Value = "Total Amount";
                    worksheet.Cells["S2"].Value = "WRBTR";

                    worksheet.Cells["T1"].Value = "Special GL Indicator";
                    worksheet.Cells["T2"].Value = "UMSKZ";

                    worksheet.Cells["U1"].Value = "Tax on Sales/Purchase Code";
                    worksheet.Cells["U2"].Value = "MWSKZ";

                    worksheet.Cells["V1"].Value = "Partner Profit Centre";
                    worksheet.Cells["V2"].Value = "PPRCTR";

                    worksheet.Cells["W1"].Value = "Baseline (due) Date";
                    worksheet.Cells["W2"].Value = "ZFBDT";

                    worksheet.Cells["X1"].Value = "Terms of payment key";
                    worksheet.Cells["X2"].Value = "ZTERM";

                    worksheet.Cells["Y1"].Value = "Assignment";
                    worksheet.Cells["Y2"].Value = "ZUONR";

                    worksheet.Cells["Z1"].Value = "HSN/SAC CODE";
                    worksheet.Cells["Z2"].Value = "HSN_SAC";

                    worksheet.Cells["AA1"].Value = "IRN Date";
                    worksheet.Cells["AA2"].Value = "Null";

                    worksheet.Cells["AB1"].Value = "IRN Flag";
                    worksheet.Cells["AB2"].Value = "Null";

                }
                var stream = new MemoryStream();
                package.SaveAs(stream);

                var fileName = $"FB60Forvol Report {DateTime.Now.ToString("dd-MM-yyyy")}.xlsx";

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        //private List<FB60ForvolModel> GetForvolReportDataFromDatabase()
        //{
        //    return GetForvolReportDataFromDatabase(null, null);
        //}

        private List<FB60ForvolModel> GetForvolReportDataFromDatabase()
        {
            List<FB60ForvolModel> reportData = new List<FB60ForvolModel>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                using (var command = new SqlCommand("trvl_GetFB60ForvolReportData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FB60ForvolModel model = new FB60ForvolModel();

                            // Handle potential conversion errors
                            try
                            {
                                model.TravelRequestId = reader["TravelRequestId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TravelRequestId"]);
                                model.InvoiceDate = reader["InvoiceDate"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["InvoiceDate"]);
                                model.InvoiceNumber = reader["InvoiceNumber"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InvoiceNumber"]);
                                model.TotalAmount = reader["TotalAmount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalAmount"]);
                                model.Emp_Name = reader["Emp_Name"] == DBNull.Value ? "" : reader["Emp_Name"].ToString();
                                model.Travel_Destination = reader["Travel_Destination"] == DBNull.Value ? "" : reader["Travel_Destination"].ToString();
                                //model.SeriesNo = reader["SeriesNo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SeriesNo"]);
                            }
                            catch (FormatException ex)
                            {
                                // Log or handle the exception (e.g., set default values)
                                Console.WriteLine($"Error converting value: {ex.Message}");
                                model.TravelRequestId = 0;
                                model.InvoiceDate = default(DateTime);
                                model.InvoiceNumber = 0;
                                model.TotalAmount = 0;
                                model.Emp_Name = string.Empty;
                                model.Travel_Destination = string.Empty;
                            }

                            reportData.Add(model);
                        }
                    }
                }
            }

            return reportData;
        }
    }
}

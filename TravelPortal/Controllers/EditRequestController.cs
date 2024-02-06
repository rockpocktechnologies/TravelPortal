using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TravelPortal.Models;
using TravelPortal.Classes;

namespace TravelPortal.Controllers
{
    public class EditRequestController : Controller
    {

        private readonly IConfiguration _configuration;

        public EditRequestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult EditTravelRequest(string requestToken)
        {

            using (SqlConnection connection = 
                new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                NewRequestModel model = new NewRequestModel();
                CommonFunctions cmn = new CommonFunctions(_configuration);

                using (SqlCommand cmd = new SqlCommand("trvl_SPGetTravelRequestEditDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RequestToken", requestToken));




                    List<MultiCityData> multiCityData = new List<MultiCityData>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.Read())
                        {
                            model.strTravelMode = reader["strTravelMode"] == DBNull.Value ? "" : reader["strTravelMode"].ToString();
                            model.strTicketBookedBy = reader["strTicketBookedBy"] == DBNull.Value ? "" : reader["strTicketBookedBy"].ToString();
                            model.strTravelDestination = reader["strTravelDestination"] == DBNull.Value ? "" : reader["strTravelDestination"].ToString();
                            model.strdivTravelType = reader["strdivTravelType"] == DBNull.Value ? "" : reader["strdivTravelType"].ToString();
                            model.strtravelFrom = reader["strtravelFrom"] == DBNull.Value ? "" : reader["strtravelFrom"].ToString();
                            model.strtravelTo = reader["strtravelTo"] == DBNull.Value ? "" : reader["strtravelTo"].ToString();
                            //model.strtravelDepartureDate = reader["strtravelDepartureDate"] == DBNull.Value ? "" : reader["strtravelDepartureDate"].ToString();
                            if (reader["strtravelDepartureDate"] != DBNull.Value)
                            {
                                model.strtravelDepartureDate = 
                                    cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strtravelDepartureDate"].ToString());

                                //DateTime departureDate;
                                //if (DateTime.TryParse(reader["strtravelDepartureDate"].ToString(), out departureDate))
                                //{
                                //    model.strtravelDepartureDate = departureDate.ToString("mm/dd/yy");
                                //    if (model.strtravelDepartureDate == "01-01-1900")
                                //    {
                                //        model.strtravelDepartureDate = "";
                                //    }
                                //}
                                //else
                                //{
                                //    model.strtravelDepartureDate = ""; // Handle invalid date format
                                //}
                            }
                            else
                            {
                                model.strtravelDepartureDate = "";
                            }
                            //model.strttravelReturnDate = reader["strttravelReturnDate"] == DBNull.Value ? "" : reader["strttravelReturnDate"].ToString();

                            if (reader["strttravelReturnDate"] != DBNull.Value)
                            {
                                model.strttravelReturnDate =
                                    cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strttravelReturnDate"].ToString());



                                //DateTime rtDate;
                                //if (DateTime.TryParse(reader["strttravelReturnDate"].ToString(), out rtDate))
                                //{
                                //    model.strttravelReturnDate = rtDate.ToString("mm/dd/yy");
                                //    if(model.strttravelReturnDate == "01-01-1900")
                                //    {
                                //        model.strttravelReturnDate = "";
                                //    }
                                //}
                                //else
                                //{
                                //    model.strttravelReturnDate = ""; // Handle invalid date format
                                //}
                            }
                            else
                            {
                                model.strttravelReturnDate = "";
                            }
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
                            
                            if (reader["strPassportIssueDate"] != DBNull.Value)
                            {
                                model.strPassportIssueDate =
                                    cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strPassportIssueDate"].ToString());
                            }
                            else
                            {
                                model.strPassportIssueDate = "";
                            }

                            if (reader["strPassportExpiryDate"] != DBNull.Value)
                            {
                                model.strPassportExpiryDate =
                                    cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strPassportExpiryDate"].ToString());
                            }
                            else
                            {

                            }
                            //    model.strPassportIssueDate = reader["strPassportIssueDate"] == DBNull.Value ? "" : reader["strPassportIssueDate"].ToString();
                            //model.strPassportExpiryDate = reader["strPassportExpiryDate"] == DBNull.Value ? "" : reader["strPassportExpiryDate"].ToString();

                            model.strNameofNominee = reader["strNameofNominee"] == DBNull.Value ? "" : reader["strNameofNominee"].ToString();
                            model.strRelationwithNominee = reader["strRelationwithNominee"] == DBNull.Value ? "" : reader["strRelationwithNominee"].ToString();

                            if (reader["strDOBofNominee"] != DBNull.Value)
                            {
                                model.strDOBofNominee =
                                    cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["strDOBofNominee"].ToString());
                            }
                            else
                            {
                                model.strDOBofNominee = "";
                            }

                            model.strGenderofNominee = reader["strGenderofNominee"] == DBNull.Value ? "" : reader["strGenderofNominee"].ToString();
                            model.IsDirectorApprovalNeeded =  reader["IsDirectorApprovalNeeded"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsDirectorApprovalNeeded"]);

                        }
                        reader.NextResult(); // Move to the next result set

                        while (reader.Read())
                        {
                            // Create a MultiCityTravelModel and populate it
                            MultiCityData city = new MultiCityData();

                            city.travelFrom = reader["SourceCity"] == DBNull.Value ? "" : reader["SourceCity"].ToString();
                            city.travelTo = reader["DestinationCity"] == DBNull.Value ? "" : reader["DestinationCity"].ToString();
                            city.travelDepartureDate = reader["DepartureDate"] == DBNull.Value ? "" : 
                                 cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(reader["DepartureDate"].ToString());

                            multiCityData.Add(city);
                        }


                    }

                    model.MultiCityData = multiCityData;


                
                    }

                    connection.Close();

                    return View("~/Views/NewRequest/Index.cshtml", model);

                }
            
        }

    }
}

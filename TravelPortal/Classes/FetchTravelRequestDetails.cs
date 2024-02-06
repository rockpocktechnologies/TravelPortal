using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using TravelPortal.Classes;
using TravelPortal.Models;

namespace TravelPortal.Classes
{
    public class FetchTravelRequestDetails
    {
        private IConfiguration _configuration;

        public FetchTravelRequestDetails(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FetchTravelRequestDetailsData(string ReceiverName,int newTravelRequestId,
            string emailTemplate, string TravelRequestToken)
        {
            string TravelHtmlForCities = "";
            CommonFunctions cmn = new CommonFunctions(_configuration);
            

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                // Create a SqlCommand to execute your SP
                using (SqlCommand command = new SqlCommand("trvl_SPGetTravelRequestDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters if your SP requires them

                    // Set parameter values
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@TravelRequestId", SqlDbType.Int)); // Assuming TravelRequestId is of type int
                    command.Parameters.Add(new SqlParameter("@TravelRequestToken", SqlDbType.VarChar, 50)); // Assuming TravelRequestToken is of type string

                    // Set parameter values
                    if (TravelRequestToken == "")
                    {
                        command.Parameters["@TravelRequestId"].Value = newTravelRequestId;
                        command.Parameters["@TravelRequestToken"].Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters["@TravelRequestId"].Value = 0;
                        command.Parameters["@TravelRequestToken"].Value = TravelRequestToken;
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Replace placeholders with actual data

                            emailTemplate = emailTemplate.Replace("{ReceiverName}", ReceiverName);
                            emailTemplate = emailTemplate.Replace("{TravelRequestNumber}", reader["TravelRequestNumber"].ToString());
                            emailTemplate = emailTemplate.Replace("{Mode}", reader["TravelMode"].ToString());
                            emailTemplate = emailTemplate.Replace("{Trip}", reader["TravelType"].ToString());
                            emailTemplate = emailTemplate.Replace("{Purpose}", reader["Purpose"].ToString());
                            emailTemplate = emailTemplate.Replace("{ProjectCode}", reader["ProjectCode"].ToString());
                            emailTemplate = emailTemplate.Replace("{Status}", reader["Status"].ToString());
                            emailTemplate = emailTemplate.Replace("{AdvanceAmountRequested}", reader["Requested_Advance_Amt"].ToString());                            
                            emailTemplate = emailTemplate.Replace("{Emp_No}", reader["Emp_No"].ToString());
                            emailTemplate = emailTemplate.Replace("{CurrencyName}", reader["CurrencyName"].ToString());
                            emailTemplate = emailTemplate.Replace("{Emp_Mobile}", reader["Emp_Mobile"].ToString());
                            //emailTemplate = emailTemplate.Replace("{PreviousApprovedBy}", reader["PreviousApprovedBy"].ToString());

                            if (reader["Status"].ToString() != "Pending with Manager 1" &&
    reader["PreviousApprovedByName"].ToString() != "")
                            {
                                string returnDateHtml = "<tr>" +
                   "    <td><strong>Previous Approved By</strong></td>" +
                   "    <td>" + reader["PreviousApprovedByName"].ToString() + "</td>" +
                   "</tr>" +
                   "<tr>";
                                emailTemplate = emailTemplate.Replace("{PreviousApprovedBy}", returnDateHtml);

                            }
                            else
                            {
                                emailTemplate = emailTemplate.Replace("{PreviousApprovedBy}", "");

                            }

                            if (reader["Requested_For"].ToString() == "2")
                            {

                                emailTemplate = emailTemplate.Replace("{EmployeeName}", "Traveller Name");
                                emailTemplate = emailTemplate.Replace("{RequestFor}", "On Behalf Of");
                                emailTemplate = emailTemplate.Replace("{SenderName}", reader["EmployeeName"].ToString());

                            }
                            else
                            {
                                emailTemplate = emailTemplate.Replace("{EmployeeName}", "Employee Name");
                                emailTemplate = emailTemplate.Replace("{RequestFor}", "Self");
                                emailTemplate = emailTemplate.Replace("{SenderName}", reader["SenderName"].ToString());

                            }

                            if (reader["TravelType"].ToString() == "One Way")
                            {


                                TravelHtmlForCities = CreateTravelCityDetails(reader["DateOfTravel"].ToString(),
                                               reader["ReturnDate"].ToString(),
                                               reader["FromPlace"].ToString(), reader["ToPlace"].ToString(), false);

                            }
                            else if (reader["TravelType"].ToString() == "Round Trip")
                            {
                                TravelHtmlForCities = CreateTravelCityDetails(reader["DateOfTravel"].ToString(),
                                            reader["ReturnDate"].ToString(),
                                            reader["FromPlace"].ToString(), reader["ToPlace"].ToString(), true);

                            }
                            else if (reader["TravelType"].ToString() == "Multi City")
                            {

                                List<MultiCityData> multiCityData = new List<MultiCityData>();


                                reader.NextResult(); // Move to the next result set


                                while (reader.Read())
                                {

                                    MultiCityData city = new MultiCityData();
                                    city.travelFrom = reader["SourceCity"] == DBNull.Value ? "" : reader["SourceCity"].ToString();
                                    city.travelTo = reader["DestinationCity"] == DBNull.Value ? "" : reader["DestinationCity"].ToString();
                                    city.travelDepartureDate = reader["DepartureDate"] == DBNull.Value ? "" : reader["DepartureDate"].ToString();

                                    multiCityData.Add(city);

                                }
                                foreach (MultiCityData item in multiCityData)
                                {
                                    TravelHtmlForCities = TravelHtmlForCities +
                                        CreateTravelCityDetails(item.travelDepartureDate,
                                        "", item.travelFrom, item.travelTo, false);
                                }
                            }



                            emailTemplate = emailTemplate.Replace("{TravelDetails}",
                                TravelHtmlForCities);


                           
                    

                        }

                    }
                }
            }


            return emailTemplate;
        }


        public string CreateTravelCityDetails(string DateOfTravel,
            string ReturnDate , string FromPlace, string ToPlace, 
            bool isReturnDateNeeded)
        {
            CommonFunctions cmn = new CommonFunctions(_configuration);

            string returnDateHtml = "";
            if (isReturnDateNeeded)
            {
                returnDateHtml = "<tr>" +
                "    <td><strong>Return Date</strong></td>" +
                "    <td>" + cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(ReturnDate) + "</td>" +
                "</tr>" +
                "<tr>";
              }

            return    "<tr>" +
                                                     "    <td><strong>Date of Travel</strong></td>" +
                                                     "    <td>" + cmn.ConvertDateFormatmmddyytoddmmyyDuringDisplay(DateOfTravel) + "</td>" +
                                                     "</tr>" +
                                                    returnDateHtml +
                                                     "    <td><strong>Source</strong></td>" +
                                                     "    <td>" + FromPlace + "</td>" +
                                                     "</tr>" +
                                                     "<tr>" +
                                                     "    <td><strong>Destination</strong></td>" +
                                                     "    <td>" + ToPlace + "</td>" +
                                                     "</tr>";
        }
    }
}

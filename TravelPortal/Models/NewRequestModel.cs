namespace TravelPortal.Models
{
    public class NewRequestModel
    {
        public string strTravelMode { get; set; }
        public string strTicketBookedBy { get; set; }
        public string strTravelDestination { get; set; }
        public string strdivTravelType { get; set; }
        public string strtravelFrom { get; set; }
        public string strtravelTo { get; set; }
        public string strtravelDepartureDate { get; set; }
        public string strttravelReturnDate { get; set; }
        public string stremployeeNo { get; set; }
        public string stremployeeName { get; set; }
        public string strgrade { get; set; }
        public string strposition { get; set; }
        public string strdepartment { get; set; }
        public string strlocation { get; set; }
        public string strmobileNumber { get; set; }
        public string strage { get; set; }
        public string strselectGender { get; set; }
        public string strselectPurpose { get; set; }
        public string strtravelDays { get; set; }
        public string strmanager1 { get; set; }
        public string strmanager2 { get; set; }
        public string strdirector { get; set; }

        public string stradvanceAmount { get; set; }
        public string strselectCurrency { get; set; }
        public string strselectProjectCode { get; set; }
        public string strcomments { get; set; }
        public string strRequestedFor { get; set; }
        public string strEmpType { get; set; }

        public string selectCurrency { get; set; }
        public string strRequestToken { get; set; }
        
        public bool isSubmitted { get; set; }

        public List<MultiCityData> MultiCityData { get; set; }
        public string strNameonPassport { get; set; }
        public string strPassportNumber { get; set; }
        public string strPassportIssueDate { get; set; }
        public string strPassportExpiryDate { get; set; }

        public bool isCreateMode { get; set; }

        public string strNameofNominee { get; set; }
        public string strRelationwithNominee { get; set; }
        public string strDOBofNominee { get; set; }

        public string strGenderofNominee { get; set; }

        public bool IsDirectorApprovalNeeded { get; set; }
    }

    public class MultiCityData
    {
        public string travelFrom { get; set; }
        public string travelTo { get; set; }
        public string travelDepartureDate { get; set; }
    }
}

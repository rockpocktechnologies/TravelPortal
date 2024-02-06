namespace TravelPortal.Models
{
    public class MyRequestsModel
    {
        public int RequestId { get; set; }
        public string RequestToken { get; set; }
        public string ApprovalToken { get; set; }
        public string TravelRequestNumber { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public string DateOfJourney { get; set; }
        public string Status { get; set; }
        public string Emp_No { get; set; }
        public string Emp_Name { get; set; }
        public string Mode { get; set; }
        public string TravelDestination { get; set; }
        public string Purpose { get; set; }
        public string RequestedFor { get; set; }

        public string Project { get; set; }
        
        public string TravelType { get; set; }

        public string Ticket_Booked_By { get; set; }

        public bool IsSubmitted { get; set; }


        
    }
}

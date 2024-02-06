namespace TravelPortal.Models
{
    public class BookingSave
    {
        public int Bookingid { get; set; } 
        public string RequestToken { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public string TravelClass { get; set; }
        public string AirlineNumber { get; set; }
        public string PnrNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Fare { get; set; }
        public string Tax { get; set; }
        public string TotalAmount { get; set; }
        public string Discount { get; set; }
        public string ServiceCharges { get; set; }
        public string ServiceTax { get; set; }
        public string NetAmount { get; set; }
        public string Attachment { get; set; }
        public string TicketStatus { get; set; }
        public string CancellationCharges { get; set; }
        public string Comments { get; set; }
        public bool IsSubmit { get; set; }
    }

}

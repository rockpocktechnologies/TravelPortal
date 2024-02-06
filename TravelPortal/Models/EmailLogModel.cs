namespace TravelPortal.Models
{
    public class EmailLogModel
    {
        public int EmailLogId { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentDate { get; set; }
        // Add more properties as needed
    }

}

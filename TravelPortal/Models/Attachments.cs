namespace TravelPortal.Models
{
    public class Attachments
    {
        public int BookingId { get; set; } // Foreign key to Booking
        public string OriginalFileName { get; set; }
        public byte[] AttachmentData { get; set; }
    }
}

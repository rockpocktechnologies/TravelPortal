
namespace TravelPortal.Models
{

    public class ApprovalLogModel
    {
        public int ApprovalLogId { get; set; }
        public int TravelRequestId { get; set; }
        public int StatusID { get; set; }
        public string NameOfReceiver { get; set; }
        public bool IsSentMailToTravelDesk { get; set; }
        public bool IsAdvanceAmountMailToAccounts { get; set; }
        public bool IsSentMailToAdmin { get; set; }
        public bool IsSentMailToInsurance { get; set; }
        public string RequestOwnerName { get; set; }
        public string RequestOwnerEmail { get; set; }
        public string RequestNumber { get; set; }
        public bool IsMakeStatusBookedWhenbySelf { get; set; }
        public string LogToken { get; set; }
        public DateTime LogDate { get; set; }
    }



}
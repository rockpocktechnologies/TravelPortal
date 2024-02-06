namespace TravelPortal.Models
{
    public class TrvlExpense
    {
        public int Id { get; set; }

        public string RequestToken { get; set; }
        public string ExpenseType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Currency { get; set; }
        public string ByCompany { get; set; }
        public string ByEmployee { get; set; }
        public string Comment { get; set; }

        public IFormFile FileData { get; set; }

        public string expenseID { get; set; }

        public bool isSubmit { get; set; }

        public string strNumber { get; set; }

        public string strReqNumber { get; set; }

        public string RequestNumber { get; set; }

        public bool IsUpdateAttachment { get; set; }

        public string strCurrencyText { get; set; }

        public string strExpenseTypeText { get; set; }

        public string strFinalAmount { get; set; }

        public bool isSendToManagerForApproval { get; set; }

        public bool isFromAccounts { get; set; }

        //public byte[] FileData { get; set; } // You can use byte[] to store binary data (e.g., file content)

    }

}

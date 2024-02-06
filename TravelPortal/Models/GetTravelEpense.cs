namespace TravelPortal.Models
{
    public class GetTravelEpense
    {
        public string strExpenseType { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strCurrency { get; set; }
        public string strByCompany { get; set; }
        public string strByEmployee { get; set; }
        public string strComment { get; set; }
        public string strfiledata { get; set; }

        public int expenseID { get; set; }

        public bool issubmitted { get; set; }
  
        public string strfilename { get; set; }

        public string ExpenseToken { get; set; }

        public string strNameEmp { get; set; }

        public string EmpNumber { get; set; }

        public string EmpGrade { get; set; }

        public string EmpMobile{ get; set; }
    }
}

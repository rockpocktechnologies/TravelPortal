namespace TravelPortal.Models
{
    public class ReportModel
    {
        // public int ID { get; set; }
        //public int Status_ID { get; set; }
        //public string Title { get; set; }
        public string TravelRequestNumber { get; set; }
        public string Departure_City { get; set; }
        public string Destination_City { get; set; }
        public int Emp_No { get; set; }
        public string Emp_Name { get; set; }
        public int Emp_Age { get; set; }
        public int Emp_Gender { get; set; }
        public string Emp_Grade { get; set; }
        public DateTime Departure_Date { get; set; }
        public DateTime Return_Date { get; set; }
        public int Travel_Days { get; set; }
        public string Emp_Passport_Name { get; set; }
        public string Emp_Passport_Int { get; set; } // Assuming it is the passport expiration date
        public DateTime Emp_Passport_Issue_Date { get; set; }
        public DateTime Emp_Passport_Expiry_Date { get; set; }
        public string NameofNominee { get; set; }
        public string RelationwithNominee { get; set; }
        public DateTime DOBofNominee { get; set; }
        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }
        // public string External_Emp_Email { get; set; }









        //public string Travel_Mode { get; set; }
        //public string Ticket_Booked_By { get; set; }
        //public string Travel_Destination { get; set; }
        //public string Travel_Type { get; set; }
        //public string Requested_For { get; set; }
        //public string Emp_Type { get; set; }

        //public string Emp_Position { get; set; }
        //public string Emp_Department { get; set; }
        //public string Emp_Location { get; set; }
        //public string Emp_Mobile { get; set; } // Assuming it is stored as a string due to potential leading zeros

        //public string Travel_Purpose { get; set; }
        //public string Emp_Project { get; set; }

        //public string Currency { get; set; }



    }
}
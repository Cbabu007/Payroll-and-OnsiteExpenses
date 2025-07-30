namespace PayrollandOnsiteExpenses.Models
{
    public class ViewAssignedEmployee
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string No { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Taluk { get; set; }
        public string District { get; set; }
        public string Pincode { get; set; }
        public string Location { get; set; }

        public string Department { get; set; }
        public string EmployeeName { get; set; } 
    }
}

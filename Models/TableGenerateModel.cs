namespace PayrollandOnsiteExpenses.Models
{
    public class TableGenerateModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public int LOPDays { get; set; }
        public string PFAcNumber { get; set; }
        public string UAN { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal Food { get; set; }
        public decimal Travel { get; set; }
        public decimal Accommodation { get; set; }
    }

}

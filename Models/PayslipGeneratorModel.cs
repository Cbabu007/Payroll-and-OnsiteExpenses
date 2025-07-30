namespace PayrollandOnsiteExpenses.Models
{
    public class PayslipGenerateModel
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string PayPeriod { get; set; }
        public DateTime DateofJoining { get; set; }
        public DateTime PayDate { get; set; }
        public int PaidDays { get; set; }
        public int LOPDays { get; set; }
        public string PFAcNumber { get; set; }
        public string UAN { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal GrossEarnings { get; set; }
        public decimal EPFContribution { get; set; }
        public decimal HealthInsurance { get; set; }
        public decimal TotalDeducation { get; set; }
        public decimal TotalNetPayable { get; set; }
        public string IssuedBy { get; set; } 
        public string Generated { get; set; } 
    }


}

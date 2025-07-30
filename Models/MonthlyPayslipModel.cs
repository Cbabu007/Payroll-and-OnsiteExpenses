namespace PayrollandOnsiteExpenses.Models
{
    public class MonthlyPayslipModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public int LOPDays { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal Food { get; set; }
        public decimal Travel { get; set; }
        public decimal Accommodation { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal GrossEarnings { get; set; }
        public decimal EPFContribution { get; set; }
        public decimal HealthInsurance { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetPayable { get; set; }
        public string PayslipFilePath { get; set; }
    }

}

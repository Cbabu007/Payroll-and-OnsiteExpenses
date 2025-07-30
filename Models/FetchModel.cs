namespace PayrollandOnsiteExpenses.Models
{
    public class FetchModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }

        public string DateOfJoining { get; set; }
        public string PFAccountNumber { get; set; }
        public string UAN { get; set; }

        public string LOPDays { get; set; }
        public string BasicSalary { get; set; }
        public string HRA { get; set; }
        public string ConveyanceAllowance { get; set; }
        public string GrossEarnings { get; set; }
        public string EPFContribution { get; set; }
        public string HealthInsurance { get; set; }
        public string TotalDeductions { get; set; }
        public string TotalNetPayable { get; set; }
    }
}

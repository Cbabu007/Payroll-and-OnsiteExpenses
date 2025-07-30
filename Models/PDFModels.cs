using System;

namespace PayrollandOnsiteExpenses.Models
{
    public class PDFModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }

        public string Month { get; set; }
        public int Year { get; set; }
        public int LOPDays { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal GrossEarnings { get; set; }
        public decimal EPFContribution { get; set; }
        public decimal HealthInsurance { get; set; }
        public decimal TotalDeducation { get; set; }
        public decimal TotalNetPayable { get; set; }

        public DateTime DateofJoining { get; set; }
        public string PFAcNumber { get; set; }
        public string UAN { get; set; }
    }
}

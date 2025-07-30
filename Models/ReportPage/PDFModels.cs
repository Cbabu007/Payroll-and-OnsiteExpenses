using System;

namespace PayrollandOnsiteExpenses.Models.ReportsPage
{
    public class PDFModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string DateOfJoining { get; set; }

        public string PayslipMonth { get; set; }
        public string PayPeriod { get; set; }
        public string PayDate { get; set; }

        public int PaidDays { get; set; }
        public int LOPDays { get; set; }

        public string PFAcNumber { get; set; }
        public string UAN { get; set; }

        public decimal Basic { get; set; }
        public decimal HRA { get; set; }
        public decimal ConveyanceAllowance { get; set; }

        public decimal EPFContribution => Math.Round(Basic * 0.12M, 2);  // 12% of basic
        public decimal HealthContribution { get; set; }

        public decimal GrossEarnings => Basic + HRA + ConveyanceAllowance;
        public decimal TotalDeductions => EPFContribution + HealthContribution;
        public decimal TotalNetPayable => GrossEarnings - TotalDeductions;

        public string SignatureFileName { get; set; }  // For storing uploaded file name
    }
}

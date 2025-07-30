namespace PayrollandOnsiteExpenses.Models
{
    public class FullPayrollDetailsModel
    {
        public int Id { get; set; } 

        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string Designation { get; set; }

        public DateTime? DateOfJoining { get; set; }

        public string PFAccountNumber { get; set; }
        public string UAN { get; set; }

        public int? LOPDays { get; set; }

        public decimal? BasicSalary { get; set; }
        public decimal? HRA { get; set; }

        public decimal Travel { get; set; }
        public decimal Food { get; set; }
        public decimal Accommodation { get; set; }

        
        public decimal GrossEarnings => (BasicSalary ?? 0) + (HRA ?? 0);
        public decimal TotalExpenses => Travel + Food + Accommodation;
    }
}

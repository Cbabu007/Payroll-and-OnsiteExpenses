using Microsoft.EntityFrameworkCore;

namespace PayrollandOnsiteExpenses.Models
{
    [Keyless] 
    public class EmployeePayslipModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string PFAcNumber { get; set; }
        public string UAN { get; set; }
        public DateTime PayDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public int DeductionPercentage { get; set; }
        public decimal GrossEarnings { get; set; }
        public decimal EPF { get; set; }
        public decimal HealthInsurance { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }

        public decimal TravelAmount { get; set; }
        public decimal FoodAmount { get; set; }
        public decimal AccommodationAmount { get; set; }

    }
}

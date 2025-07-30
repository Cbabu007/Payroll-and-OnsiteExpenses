using System;
using System.ComponentModel.DataAnnotations;


namespace PayrollandOnsiteExpenses.Models
{
    public class PayrollModel
    {
        public int Id { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        public string EmployeeID { get; set; }


        public string Month { get; set; }
        public int Year { get; set; }

        public string MonthYear => $"{Month} {Year}";

        [Required]
        public decimal BasicSalary { get; set; }

        public bool IsMetroCity { get; set; }

        public decimal HRA { get; set; }

        public decimal Allowances { get; set; }

        public int DeductionPercentage { get; set; }

        public decimal Deductions { get; set; }

        public decimal NetSalary { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime GeneratedDate { get; set; }


    }
}

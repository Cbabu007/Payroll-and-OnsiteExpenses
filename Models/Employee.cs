using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollandOnsiteExpenses.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeID { get; set; }
        public string? PhotoPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? AltPhoneNumber { get; set; }
        public DateTime? DOB { get; set; }  

        public string? DoorNo { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Taluk { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? Country { get; set; }

        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; }

        public List<EmployeeQualification> Qualifications { get; set; }
    }


    public class EmployeeQualification
    {
        [Key]
        public int QualificationID { get; set; }
        public string EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
        public string QualificationType { get; set; }
        public string Subject { get; set; }
        public string CertificateNumber { get; set; }
        public string? CertificateDocPath { get; set; }
    }

}

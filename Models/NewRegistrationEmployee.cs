namespace PayrollandOnsiteExpenses.Models
{
    public class NewRegistrationEmployee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string DoorNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Location { get; set; }
        public string Taluk { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Country { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeUsername { get; set; }
        public string EmployeePassword { get; set; }
        public string Role { get; set; }
        public IFormFile Photo { get; set; } 
    }

}

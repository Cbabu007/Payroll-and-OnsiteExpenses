namespace PayrollandOnsiteExpenses.Models
{
    public class PendingApprovalsModel
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string ProjectName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}

namespace PayrollandOnsiteExpenses.Models
{
    public class ExpenseApprovalSummary
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? UploadFilePath { get; set; }
        public string? ProjectName { get; set; }
        public decimal? TravelAmount { get; set; }
        public string? TravelStatus { get; set; }
        public decimal? FoodAmount { get; set; }
        public string? FoodStatus { get; set; }
        public decimal? AccommodationAmount { get; set; }
        public string? AccommodationStatus { get; set; }
        public string? CloseStatus { get; set; }
        
    }
}

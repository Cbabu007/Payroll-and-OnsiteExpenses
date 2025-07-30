public class ExpensesClaimReport
{
    public int Id { get; set; }
    public string EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } 
    public string ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } 
    public string? UploadFilePath { get; set; } 

    public string Month { get; set; }
    public string Year { get; set; }
}

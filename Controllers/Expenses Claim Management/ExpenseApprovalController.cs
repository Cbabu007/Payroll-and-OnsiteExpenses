using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseApprovalController : ControllerBase
    {
        private readonly PayrollDbContext _context;

        public ExpenseApprovalController(PayrollDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetSummary")]
        public IActionResult GetSummary()
        {
            var data = _context.ExpenseApprovalSummary.ToList();
            return Ok(data);
        }

        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromForm] int id, [FromForm] string category, [FromForm] string status)
        {
            var record = _context.ExpenseApprovalSummary.FirstOrDefault(x => x.Id == id);
            if (record == null) return NotFound();

            if (record.CloseStatus == "Completed")
                return Ok(new { success = false, message = "Already completed. No further changes allowed." });

            switch (category.ToLower())
            {
                case "travel":
                    record.TravelStatus = status;
                    break;
                case "food":
                    record.FoodStatus = status;
                    break;
                case "accommodation":
                    record.AccommodationStatus = status;
                    break;
                default:
                    return BadRequest("Invalid category");
            }

            if (record.TravelStatus == "OK" && record.FoodStatus == "OK" && record.AccommodationStatus == "OK")
                record.CloseStatus = "Completed";
            else
                record.CloseStatus = "Incomplete";

            _context.SaveChanges();
            return Ok(new { success = true });
        }

        [HttpPost("GenerateSummary")]
        public IActionResult GenerateSummary()
        {
            
            var existingCompleted = _context.ExpenseApprovalSummary
                .Where(e => e.CloseStatus == "Completed")
                .ToList();

            
            var groupedData = _context.ExpensesClaimReports
                .GroupBy(e => new { e.EmployeeId, e.UploadFilePath, e.ProjectName })
                .Select(g => new ExpenseApprovalSummary
                {
                    EmployeeId = g.Key.EmployeeId ?? string.Empty,
                    UploadFilePath = g.Key.UploadFilePath ?? string.Empty,
                    ProjectName = g.Key.ProjectName ?? string.Empty,
                    TravelAmount = g.Where(e => e.ExpenseType == "Travel").Sum(e => (decimal?)e.Amount) ?? 0,
                    FoodAmount = g.Where(e => e.ExpenseType == "Food").Sum(e => (decimal?)e.Amount) ?? 0,
                    AccommodationAmount = g.Where(e => e.ExpenseType == "Accommodation").Sum(e => (decimal?)e.Amount) ?? 0,
                    TravelStatus = "Pending",
                    FoodStatus = "Pending",
                    AccommodationStatus = "Pending",
                    CloseStatus = "Incomplete"
                }).ToList();

            
            var toDelete = _context.ExpenseApprovalSummary
                .Where(e => e.CloseStatus != "Completed")
                .ToList();
            _context.ExpenseApprovalSummary.RemoveRange(toDelete);

           
            foreach (var record in existingCompleted)
            {
                _context.Entry(record).State = EntityState.Unchanged;
            }

            groupedData = groupedData
                .Where(newItem => !existingCompleted.Any(existing =>
                    existing.EmployeeId == newItem.EmployeeId &&
                    existing.UploadFilePath == newItem.UploadFilePath &&
                    existing.ProjectName == newItem.ProjectName))
                .ToList();

            
            _context.ExpenseApprovalSummary.AddRange(groupedData);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Summary generated successfully!" });
        }

        [HttpPost("DeleteExpense")]
        public IActionResult DeleteExpense([FromForm] int id)
        {
            var record = _context.ExpenseApprovalSummary.FirstOrDefault(x => x.Id == id);
            if (record == null)
                return NotFound();

            _context.ExpenseApprovalSummary.Remove(record);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

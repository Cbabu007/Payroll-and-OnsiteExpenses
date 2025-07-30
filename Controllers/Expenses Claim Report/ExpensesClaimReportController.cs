using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PayrollandOnsiteExpenses.Controllers
{
    [Route("ExpensesClaimReport")]
    public class ExpensesClaimReportController : Controller
    {
        private readonly PayrollDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ExpensesClaimReportController(PayrollDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        
        [HttpGet("GetProjects")]
        public IActionResult GetProjects()
        {
            var projects = _context.ProjectTable
                .Select(p => new { projectId = p.Id, projectName = p.ProjectName })
                .ToList();

            return Json(projects);
        }

        
        [HttpPost("Submit")]
        public async Task<IActionResult> Submit(
            [FromForm] string EmployeeId,
            [FromForm] int ProjectId,
            [FromForm] string Date,
            [FromForm] IFormFile FileUpload,
            [FromForm] string ExpensesJson)
        {
            try
            {
                
                var project = await _context.ProjectTable.FindAsync(ProjectId);
                if (project == null)
                {
                    return Json(new { success = false, message = "Invalid project selected." });
                }

                
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var expenses = JsonSerializer.Deserialize<List<ExpensesInputModel>>(ExpensesJson, options);

                if (expenses == null || !expenses.Any())
                {
                    return Json(new { success = false, message = "No expenses provided." });
                }

                
                string filePath = null;
                if (FileUpload != null && FileUpload.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "expenses");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(FileUpload.FileName);
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileUpload.CopyToAsync(stream);
                    }
                }

               
                foreach (var expense in expenses)
                {
                    var report = new ExpensesClaimReport
                    {
                        EmployeeId = EmployeeId,
                        ProjectId = ProjectId,
                        ProjectName = project.ProjectName, 
                        ExpenseType = expense.ExpenseType,
                        Amount = Convert.ToDecimal(expense.Amount),
                        CreatedAt = Convert.ToDateTime(Date),
                        UploadFilePath = filePath
                    };

                    _context.ExpensesClaimReports.Add(report);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner Exception: " + ex.InnerException.Message;
                }

                return Json(new { success = false, message = errorMessage });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;

namespace PayrollandOnsiteExpenses.Controllers.Payroll_Management
{
    public class GeneratePayrollController : Controller
    {
        private readonly PayrollDbContext _context;

        public GeneratePayrollController(PayrollDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GeneratePayroll()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            var departments = _context.Employees
                .Select(e => e.Role)
                .Distinct()
                .ToList();

            return Json(departments);
        }

        [HttpGet]
        public IActionResult GetEmployeesByRole(string role)
        {
            var employees = _context.Employees
                .Where(e => e.Role == role)
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    EmployeeID = e.EmployeeID
                })
                .ToList();

            return Json(employees);
        }




        [HttpPost("GeneratePayroll/GeneratePayroll")]
        public async Task<IActionResult> GeneratePayroll([FromBody] PayrollModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid input", modelState = ModelState });
            }

            try
            {
                
                model.BasicSalary = GetBasicSalary(model.Department);
                decimal fixedHRA = GetFixedHRA(model.Department);
                decimal hraCalc = model.IsMetroCity ? model.BasicSalary * 0.5m : model.BasicSalary * 0.4m;
                model.HRA = Math.Max(fixedHRA, hraCalc);
                model.Allowances = GetAllowance(model.Department, model.BasicSalary);
                model.Deductions = GetDeductionAmount(model.DeductionPercentage, model.BasicSalary);
                model.NetSalary = model.BasicSalary + model.HRA + model.Allowances - model.Deductions;

                if (model.GeneratedDate == DateTime.MinValue)
                {
                    model.GeneratedDate = DateTime.Now;
                }

               

                _context.PayrollTable.Add(model);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payroll submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }

        }

        private decimal GetBasicSalary(string role) => role switch
        {
            "Project Manager" => 80000,
            "Civil Engineer" => 35000,
            "Site Engineer" => 28000,
            "Site Supervisor" => 20000,
            "Architect" => 50000,
            "Quantity Surveyor" => 35000,
            "Safety Officer" => 30000,
            "Office Staff (HR/Admin)" => 20000,
            "Mason" => 20000,
            "Carpenter" => 15000,
            "Electrician" => 15000,
            "Plumber" => 15000,
            "Painter" => 15000,
            "Helper / Laborers" => 12000,
            _ => 0
        };

        private decimal GetFixedHRA(string role) => role switch
        {
            "Project Manager" => 10000,
            "Civil Engineer" => 5000,
            "Site Engineer" => 4000,
            "Site Supervisor" => 3000,
            "Architect" => 6000,
            "Quantity Surveyor" => 5000,
            "Safety Officer" => 4000,
            "Office Staff (HR/Admin)" => 3000,
            "Mason" => 2000,
            "Carpenter" => 2000,
            "Electrician" => 2000,
            "Plumber" => 2000,
            "Painter" => 2000,
            "Helper / Laborers" => 1500,
            _ => 0
        };

        private decimal GetAllowance(string role, decimal basic)
        {
            var highRoles = new[] {
                "Project Manager", "Civil Engineer", "Site Engineer", "Site Supervisor",
                "Architect", "Quantity Surveyor", "Safety Officer", "Office Staff (HR/Admin)"
            };
            return highRoles.Contains(role) ? basic * 0.15m : basic * 0.10m;
        }

        private decimal GetDeductionAmount(int percent, decimal basic)
        {
            if (percent >= 1 && percent <= 5) return basic * 0.01m;
            if (percent > 5 && percent <= 10) return basic * 0.02m;
            if (percent > 10 && percent <= 15) return basic * 0.15m;
            if (percent > 15 && percent <= 30) return basic * 0.75m;
            return 0;
        }
    }
}

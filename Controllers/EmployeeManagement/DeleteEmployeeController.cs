using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollandOnsiteExpenses.Data;

namespace PayrollandOnsiteExpenses.Controllers
{
    [Route("DeleteEmployee")]
    public class DeleteEmployeeController : Controller
    {
        private readonly PayrollDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DeleteEmployeeController(PayrollDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("Find")]
        public IActionResult Find(string employeeId)
        {
            var employee = _context.Employees
                .Include(e => e.Qualifications)
                .FirstOrDefault(e => e.EmployeeID == employeeId);

            if (employee == null)
                return NotFound("Employee not found.");

            return Ok(new
            {
                employee.EmployeeID,
                employee.FirstName,
                employee.LastName,
                employee.PhoneNumber,
                employee.AltPhoneNumber,
                DOB = employee.DOB?.ToString("yyyy-MM-dd"),
                employee.Address1,
                employee.Address2,
                employee.Taluk,
                employee.District,
                employee.State,
                employee.Pincode,
                employee.Country,
                employee.Email,
                employee.Username,
                employee.Role,
                employee.PhotoPath,
                Qualifications = employee.Qualifications.Select(q => new
                {
                    q.QualificationID,
                    q.QualificationType,
                    q.Subject,
                    q.CertificateNumber,
                    q.CertificateDocPath
                })
            });

        }

        [HttpPost("RemoveQualification/{id}")]
        public IActionResult RemoveQualification(int id)
        {
            var qualification = _context.EmployeeQualifications.Find(id);
            if (qualification == null)
                return NotFound("Qualification not found.");

            _context.EmployeeQualifications.Remove(qualification);
            _context.SaveChanges();
            return Ok("Qualification removed.");
        }

        [HttpPost("Delete/{employeeId}")]
        public IActionResult Delete(string employeeId)
        {
            var employee = _context.Employees
                .Include(e => e.Qualifications)
                .FirstOrDefault(e => e.EmployeeID == employeeId);

            if (employee == null)
                return NotFound("Employee not found.");

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return Ok("Employee deleted.");
        }
    }
}

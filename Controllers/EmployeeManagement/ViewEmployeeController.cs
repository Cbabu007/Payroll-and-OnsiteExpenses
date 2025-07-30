using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollandOnsiteExpenses.Data;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers
{
    [Route("ViewEmployee")]
    public class ViewEmployeeController : Controller
    {
        private readonly PayrollDbContext _context;

        public ViewEmployeeController(PayrollDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("Find")]
        public IActionResult Find(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return BadRequest("Employee ID is required.");

            var employee = _context.Employees
                .Include(e => e.Qualifications)
                .FirstOrDefault(e => e.EmployeeID == employeeId);

            if (employee == null)
                return NotFound("Employee not found.");

            var result = new
            {
                employee.EmployeeID,
                employee.FirstName,
                employee.LastName,
                employee.PhoneNumber,
                employee.AltPhoneNumber,
                dob = employee.DOB?.ToString("yyyy-MM-dd"),
                employee.Email,
                employee.Role,
                employee.DoorNo,
                employee.Address1,
                employee.Address2,
                employee.Taluk,
                employee.District,
                employee.State,
                employee.Pincode,
                employee.Country,

                PhotoPath = string.IsNullOrEmpty(employee.PhotoPath) ? "no-image.png" : employee.PhotoPath,




                qualifications = employee.Qualifications.Select(q => new
                {
                    q.QualificationType,
                    q.Subject,
                    q.CertificateNumber,
                    q.CertificateDocPath
                })
            };

            return Ok(result);
        }

       
        [HttpGet("FindAll")]
        public IActionResult FindAll()
        {
            var employees = _context.Employees
                .Include(e => e.Qualifications)
                .ToList();

            var results = employees.Select(employee => new
            {
                employee.EmployeeID,
                employee.FirstName,
                employee.LastName,
                employee.PhoneNumber,
                employee.AltPhoneNumber,
                dob = employee.DOB?.ToString("yyyy-MM-dd"),
                employee.Email,
                employee.Role,
                employee.DoorNo,
                employee.Address1,
                employee.Address2,
                employee.Taluk,
                employee.District,
                employee.State,
                employee.Pincode,
                employee.Country,


                PhotoPath = string.IsNullOrEmpty(employee.PhotoPath) ? "no-image.png" : employee.PhotoPath,





                qualifications = employee.Qualifications.Select(q => new
                {
                    q.QualificationType,
                    q.Subject,
                    q.CertificateNumber,
                    q.CertificateDocPath
                })
            });

            return Ok(results);
        }
    }
}

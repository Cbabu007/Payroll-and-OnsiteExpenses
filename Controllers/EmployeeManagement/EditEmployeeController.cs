using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;

namespace PayrollandOnsiteExpenses.Controllers.EmployeeManagement
{
    [Route("EditEmployee")]
    public class EditEmployeeController : Controller
    {
        private readonly PayrollDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditEmployeeController(PayrollDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        
        [HttpGet("GetEmployeeIDs")]
        public IActionResult GetEmployeeIDs()
        {
            var ids = _context.Employees.Select(e => e.EmployeeID).ToList();
            return Ok(ids);
        }

        
        [HttpGet("GetById")]
        public IActionResult GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Employee ID is required.");

            var employee = _context.Employees
                .Include(e => e.Qualifications)
                .FirstOrDefault(e => e.EmployeeID == id);

            if (employee == null)
                return NotFound();

            return Ok(new
            {
                employee.EmployeeID,
                employee.FirstName,
                employee.LastName,
                employee.PhoneNumber,
                employee.AltPhoneNumber,
                DOB = employee.DOB?.ToString("yyyy-MM-dd"),
                employee.DoorNo,
                employee.Address1,
                employee.Address2,
                employee.Taluk,
                employee.District,
                employee.State,
                employee.Pincode,
                employee.Country,
                employee.Email,
                employee.Username,
                employee.Password,
                employee.Role,
                employee.PhotoPath,
                Qualifications = employee.Qualifications?.Select(q => new
                {
                    q.QualificationType,
                    q.Subject,
                    q.CertificateNumber,
                    q.CertificateDocPath
                }).ToList()
            });
        }

        
        [HttpPost("Update")]
        public async Task<IActionResult> Update(Employee employee)
        {
            var updateType = Request.Form["UpdateType"];
            var existing = await _context.Employees
                .Include(e => e.Qualifications)
                .FirstOrDefaultAsync(e => e.EmployeeID == employee.EmployeeID);

            if (existing == null)
                return Json(new { success = false, message = "Employee not found." });

            if (updateType == "qualification")
            {
                var qualification = existing.Qualifications.FirstOrDefault();
                if (qualification != null)
                {
                    qualification.QualificationType = Request.Form["QualificationType"];
                    qualification.Subject = Request.Form["Subject"];
                    qualification.CertificateNumber = Request.Form["CertificateNumber"];

                    var certFile = Request.Form.Files["CertificateDoc"];
                    if (certFile != null && certFile.Length > 0)
                    {
                        var path = Path.Combine("uploads", "certs", Guid.NewGuid() + Path.GetExtension(certFile.FileName));
                        var fullPath = Path.Combine(_env.WebRootPath, path);
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        await certFile.CopyToAsync(stream);
                        qualification.CertificateDocPath = "/" + path.Replace("\\", "/");
                    }
                }
            }
            else
            {
               
                existing.FirstName = employee.FirstName;
                existing.LastName = employee.LastName;
                existing.PhoneNumber = employee.PhoneNumber;
                existing.AltPhoneNumber = employee.AltPhoneNumber;
                existing.DOB = employee.DOB;
                existing.DoorNo = employee.DoorNo;
                existing.Address1 = employee.Address1;
                existing.Address2 = employee.Address2;
                existing.Taluk = employee.Taluk;
                existing.District = employee.District;
                existing.State = employee.State;
                existing.Pincode = employee.Pincode;
                existing.Country = employee.Country;
                existing.Email = employee.Email;
                existing.Username = employee.Username;
                existing.Password = employee.Password;
                existing.Role = employee.Role;

               
                var photoFile = Request.Form.Files["Photo"];
                if (photoFile != null && photoFile.Length > 0)
                {
                    var path = Path.Combine("uploads", "photos", Guid.NewGuid() + Path.GetExtension(photoFile.FileName));
                    var fullPath = Path.Combine(_env.WebRootPath, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await photoFile.CopyToAsync(stream);
                    existing.PhotoPath = "/" + path.Replace("\\", "/");
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Employee updated successfully!" });
        }
    }
}

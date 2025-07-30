// CONTROLLER: AddEmployeeController.cs
using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollandOnsiteExpenses.Controllers.EmployeeManagement
{
    public class AddEmployeeController : Controller
    {
        private readonly PayrollDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AddEmployeeController(PayrollDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        [Route("AddEmployee/Create")]
        public async Task<IActionResult> Create(Employee employee)
        {
            bool idExists = _context.Employees.Any(e => e.EmployeeID == employee.EmployeeID);
            if (idExists)
            {
                TempData["Error"] = $"Employee ID '{employee.EmployeeID}' is already in use. Please generate a new ID.";
                return RedirectToAction("EmployeeManagement", "Admin");
            }

            var photo = HttpContext.Request.Form.Files["Photo"];
            if (photo != null && photo.Length > 0)
            {
                var photoPath = Path.Combine("uploads/photos", Guid.NewGuid() + Path.GetExtension(photo.FileName));
                var fullPath = Path.Combine(_env.WebRootPath, photoPath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                employee.PhotoPath = "/" + photoPath;
            }

            var qualificationTypes = HttpContext.Request.Form["QualificationType"];
            var subjects = HttpContext.Request.Form["Subject"];
            var certificateNumbers = HttpContext.Request.Form["CertificateNumber"];
            var certFiles = HttpContext.Request.Form.Files.Where(f => f.Name == "CertificateDoc").ToList();

            var qualifications = new List<EmployeeQualification>();

            for (int i = 0; i < qualificationTypes.Count; i++)
            {
                var qualification = new EmployeeQualification
                {
                    EmployeeID = employee.EmployeeID,
                    QualificationType = qualificationTypes[i],
                    Subject = subjects[i],
                    CertificateNumber = certificateNumbers[i]
                };

                if (certFiles.Count > i && certFiles[i] != null && certFiles[i].Length > 0)
                {
                    var certPath = Path.Combine("uploads/certs", Guid.NewGuid() + Path.GetExtension(certFiles[i].FileName));
                    var fullCertPath = Path.Combine(_env.WebRootPath, certPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullCertPath)!);
                    using (var stream = new FileStream(fullCertPath, FileMode.Create))
                    {
                        await certFiles[i].CopyToAsync(stream);
                    }
                    qualification.CertificateDocPath = "/" + certPath;
                }

                qualifications.Add(qualification);
            }

            employee.Qualifications = qualifications;
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Employee registered successfully!";
            return RedirectToAction("EmployeeManagement", "Admin");
        }

        [HttpGet]
        [Route("AddEmployee/GetNextEmployeeID")]
        public IActionResult GetNextEmployeeID()
        {
            var lastId = _context.Employees
                .OrderByDescending(e => e.EmployeeID)
                .Select(e => e.EmployeeID)
                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastId) && lastId.StartsWith("EMP"))
            {
                var numericPart = lastId.Substring(3);
                int.TryParse(numericPart, out nextNumber);
                nextNumber++;
            }

            string nextId = "EMP" + nextNumber.ToString("D3");
            return Json(nextId);
        }
    }
}

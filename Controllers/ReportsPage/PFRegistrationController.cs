using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    public class PFRegistrationController : Controller
    {
        private readonly PayrollDbContext _context;

        public PFRegistrationController(PayrollDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Employees()
        {
            var employees = _context.Employees
                .Select(e => new {
                    e.EmployeeID,
                    FullName = e.FirstName + " " + e.LastName
                }).ToList();

            return Json(employees);
        }

        [HttpPost]
        public IActionResult Register([FromBody] PFRegistration model)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeID == model.EmployeeID);
            if (employee != null)
            {
                model.EmployeeName = employee.FirstName + " " + employee.LastName;
                _context.PFRegistrations.Add(model);
                _context.SaveChanges();
                return Json(new { success = true, message = "Registered successfully!" });
            }
            return Json(new { success = false, message = "Employee not found!" });
        }

        [HttpPost]
        public IActionResult Update([FromBody] PFRegistration model)
        {
            var existing = _context.PFRegistrations.FirstOrDefault(r => r.EmployeeID == model.EmployeeID);
            if (existing != null)
            {
                existing.DateOfJoining = model.DateOfJoining;
                existing.PFAcNumber = model.PFAcNumber;
                existing.UAN = model.UAN;
                _context.SaveChanges();
                return Json(new { success = true, message = "Updated successfully!" });
            }
            return Json(new { success = false, message = "Record not found for update!" });
        }

        [HttpPost]
        public IActionResult Clear([FromBody] PFRegistration model)
        {
            string employeeId = model.EmployeeID;
            var record = _context.PFRegistrations.FirstOrDefault(r => r.EmployeeID == employeeId);

            if (record != null)
            {
                _context.PFRegistrations.Remove(record);
                _context.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully!" });
            }

            return Json(new { success = false, message = "Record not found!" });
        }


        [HttpGet]
        public IActionResult GetPFByEmployee(string employeeId)
        {
            var records = _context.PFRegistrations
                .Where(r => r.EmployeeID == employeeId)
                .Select(r => new {
                    r.Id,
                    r.EmployeeID,
                    r.PFAcNumber,
                    r.UAN
                }).ToList();

            return Json(records);
        }

       
        [HttpPost]
        public IActionResult DeleteById([FromBody] int id)
        {
            var record = _context.PFRegistrations.FirstOrDefault(r => r.Id == id);
            if (record != null)
            {
                _context.PFRegistrations.Remove(record);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Record not found!" });
        }

    }
}

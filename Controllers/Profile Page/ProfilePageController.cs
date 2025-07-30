using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers.ProfilePage
{
    public class ProfilePageController : Controller
    {
        private readonly PayrollDbContext _context;

        public ProfilePageController(PayrollDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var empId = HttpContext.Session.GetString("EmployeeId");
            var password = HttpContext.Session.GetString("Password");

            if (string.IsNullOrEmpty(empId) || string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Index", "Home");
            }

            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeID == empId && e.Password == password);

            if (employee == null)
            {
                return Json(new { success = false, message = "No employee found." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    employee.PhotoPath,
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
                    employee.Email
                }
            });
        }

        [HttpPost]
        public IActionResult UpdateProfile([FromBody] Employee updated)
        {
            var empId = HttpContext.Session.GetString("EmployeeId");
            var password = HttpContext.Session.GetString("Password");

            if (string.IsNullOrEmpty(empId) || string.IsNullOrEmpty(password))
                return Json(new { success = false, message = "Unauthorized." });

            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeID == empId && e.Password == password);

            if (employee == null)
                return Json(new { success = false, message = "Employee not found." });

            
            employee.FirstName = updated.FirstName;
            employee.LastName = updated.LastName;
            employee.PhoneNumber = updated.PhoneNumber;
            employee.AltPhoneNumber = updated.AltPhoneNumber;
            employee.DOB = updated.DOB;
            employee.Email = updated.Email;

            employee.DoorNo = updated.DoorNo;
            employee.Address1 = updated.Address1;
            employee.Address2 = updated.Address2;
            employee.Taluk = updated.Taluk;
            employee.District = updated.District;
            employee.State = updated.State;
            employee.Pincode = updated.Pincode;
            employee.Country = updated.Country;

            _context.SaveChanges();

            return Json(new { success = true });
        }


    }
}

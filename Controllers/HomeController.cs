using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Models;
using PayrollandOnsiteExpenses.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace PayrollandOnsiteExpenses.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PayrollDbContext _context;

        public HomeController(ILogger<HomeController> logger, PayrollDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        
        public IActionResult Index()
        {
            return View();
        }

       
        [HttpPost]
        public IActionResult Index(User model)
        {
            if (ModelState.IsValid)
            {
                
                if (model.Username == "ABCadmin")
                {
                    var admin = _context.Users
                        .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                    if (admin != null)
                        return RedirectToAction("Dashboard", "Admin");

                    ViewBag.Error = "Invalid admin credentials.";
                    return View(model);
                }

                
                var employee = _context.Employees
                    .FirstOrDefault(e => e.EmployeeID == model.Username && e.Password == model.Password);

                if (employee != null)
                {
                    
                    HttpContext.Session.SetString("EmployeeId", employee.EmployeeID);
                    HttpContext.Session.SetString("Password", employee.Password);
                    HttpContext.Session.SetString("Role", employee.Role);

                    return RedirectToAction("Dashboard", "Employee");
                }

                ViewBag.Error = "Invalid employee credentials.";
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

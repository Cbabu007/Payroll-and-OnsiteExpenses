using Microsoft.AspNetCore.Mvc;

    public class AdminController : Controller
    {
        
        public IActionResult Dashboard()
        {
            return View("AdminDashboard");
        }

        
        public IActionResult EmployeeManagement()
        {
            return View("EmployeeManagementPage");
        }

        
        public IActionResult ProjectManagement()
        {
            return View("ProjectManagementPage");
        }

       
        public IActionResult PayrollManagement()
        {
            return View("PayrollManagementPage");
        }

        
        public IActionResult ExpensesClaimManagement()
        {
            return View("ExpensesClaimManagementPage");
        }

        
        public IActionResult Reports()
        {
            return View("ReportsPage");
        }

        
        public IActionResult EmployeeSalaryDetailsPage()
        {
            return View("EmployeeSalaryDetailsPage");
        }

        
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }

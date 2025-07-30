using Microsoft.AspNetCore.Mvc;


    public class EmployeeController : Controller
    {
       
        public IActionResult Dashboard()
        {
            return View("UserDashboard");
        }

        
        public IActionResult ProfilePage()
        {
            return View("ProfilePage");
        }

        
        public IActionResult PayslipViewer()
        {
            return View("PayslipViewer");
        }

        
        public IActionResult ExpensesClaimReport()
        {
            return View("ExpensesClaimReport");
        }

       
        public IActionResult ClaimStatusViewer()
        {
            return View("ClaimStatusViewer");
        }

        
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }


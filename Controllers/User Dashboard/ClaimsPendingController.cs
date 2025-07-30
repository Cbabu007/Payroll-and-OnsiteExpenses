using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;

namespace PayrollandOnsiteExpenses.Controllers.User_Dashboard
{
    public class ClaimsPendingController : Controller
    {
        private readonly PayrollDbContext _context;

        public ClaimsPendingController(PayrollDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("UserDashboard/GetProjectStatusSummary")]
        public JsonResult GetProjectStatusSummary(string employeeId)
        {
            var data = _context.ExpenseApprovalSummary
                .Where(p => p.EmployeeId == employeeId)
                .Select(p => new
                {
                    ProjectName = p.ProjectName,
                    Status = (p.TravelStatus == "Pending" || p.FoodStatus == "Pending" || p.AccommodationStatus == "Pending") ? "Pending" : "Ok"
                })
                .ToList();

            return Json(data);
        }
    }
}

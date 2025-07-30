using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;

namespace PayrollandOnsiteExpenses.Controllers.User_Dashboard
{
    public class TotalClaimedController : Controller
    {
        private readonly PayrollDbContext _context;

        public TotalClaimedController(PayrollDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("UserDashboard/GetPendingApprovalProjects")]
        public JsonResult GetPendingApprovalProjects(string employeeId)
        {
            var data = _context.PendingApprovals
                .Where(p => p.EmployeeID == employeeId)
                .Select(p => new
                {
                    ProjectName = p.ProjectName,
                    Total = p.TotalAmount
                })
                .ToList();

            return Json(data);
        }
    }
}

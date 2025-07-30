using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers.User_Dashboard
{
    public class EmployeeUserDashboardController : Controller
    {
        private readonly PayrollDbContext _context;

        public EmployeeUserDashboardController(PayrollDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public JsonResult GetPendingApprovalProjects(string employeeId)
        {
            var data = _context.PendingApprovals
                .Where(p => p.EmployeeID == employeeId)
                .Select(p => new
                {
                    ProjectName = p.ProjectName,
                    Total = p.TotalAmount
                }).ToList();

            return Json(data);
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

        
        [HttpGet]
        [Route("UserDashboard/GetAssignedProjects")]
        public JsonResult GetAssignedProjects(string employeeId)
        {
            var data = (from ap in _context.AssignedProjectEmployees
                        join pt in _context.ProjectTable on ap.ProjectName equals pt.ProjectName
                        where ap.EmployeeId == employeeId
                        select new
                        {
                            ap.ProjectName,
                            pt.StartDate,
                            pt.EndDate
                        }).ToList();

            var result = data.Select((item, index) => new
            {
                Serial = index + 1,
                item.ProjectName,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                EndDate = item.EndDate.ToString("yyyy-MM-dd")
            }).ToList();

            return Json(result);
        }
    }
}

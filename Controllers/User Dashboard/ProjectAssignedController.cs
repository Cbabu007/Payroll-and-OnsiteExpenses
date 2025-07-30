using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers.User_Dashboard
{
    public class ProjectAssignedController : Controller
    {
        private readonly PayrollDbContext _context;

        public ProjectAssignedController(PayrollDbContext context)
        {
            _context = context;
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

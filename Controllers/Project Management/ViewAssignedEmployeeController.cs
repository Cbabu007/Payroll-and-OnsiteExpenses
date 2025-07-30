using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System.Linq;
using System.Collections.Generic;

namespace PayrollandOnsiteExpenses.Controllers.Project_Management
{
    public class ViewProjectListController : Controller
    {
        private readonly PayrollDbContext _context;

        public ViewProjectListController(PayrollDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public JsonResult GetAllProjectNames()
        {
            var list = _context.ProjectTable
                .Select(p => new
                {
                    projectID = p.Id,
                    projectName = p.ProjectName
                })
                .ToList();

            return Json(list);
        }

        
        [HttpGet]
        public JsonResult GetProjectDetails(int projectId)
        {
            var project = _context.ProjectTable.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                return Json(new List<ViewAssignedEmployee>());

            
            var employees = _context.AssignedProjectEmployees
                .Where(x => x.ProjectName == project.ProjectName)
                .Select(x => new ViewAssignedEmployee
                {
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Department = x.Department,
                    EmployeeName = x.EmployeeName,

                    No = project.No,
                    Address1 = project.Address1,
                    Address2 = project.Address2,
                    Taluk = project.Taluk,
                    District = project.District,
                    Pincode = project.Pincode,
                    Location = project.Location
                })
                .ToList();

            return Json(employees);
        }
    }
}

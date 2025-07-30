using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Data;
using PayrollandOnsiteExpenses.Models;
using System.Linq;

namespace PayrollandOnsiteExpenses.Controllers.Api
{
    [Route("api/AssignedProjectEmployeeData")]

    [ApiController]
    public class AssignedProjectEmployeeDataController : ControllerBase
    {
        private readonly PayrollDbContext _context;

        public AssignedProjectEmployeeDataController(PayrollDbContext context)
        {
            _context = context;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] List<AssignedProjectEmployee> assignedList)
        {
            if (assignedList == null || assignedList.Count == 0)
            {
                return BadRequest(new { success = false, message = "No data to save." });
            }

            await _context.AssignedProjectEmployees.AddRangeAsync(assignedList);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Employees successfully assigned to the project." });
        }


        
        [HttpGet("GetProjects")]
public IActionResult GetProjects()
{
    var projectNames = _context.ProjectTable  
        .Select(p => p.ProjectName)
        .Distinct()
        .ToList();

    return Ok(projectNames);
}


        
        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {
            var roles = _context.Employees
                .Select(e => e.Role)
                .Distinct()
                .ToList();

            return Ok(roles);
        }

       
        [HttpGet("GetEmployeesByDepartment")]
        public IActionResult GetEmployeesByDepartment([FromQuery] string role)
        {
            var employees = _context.Employees
                .Where(e => e.Role == role)
                .Select(e => new
                {
                    employeeID = e.EmployeeID,
                    fullName = e.FirstName + " " + e.LastName
                })
                .ToList();

            return Ok(employees);
        }


    }
}

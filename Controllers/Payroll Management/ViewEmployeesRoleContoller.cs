using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace PayrollandOnsiteExpenses.Controllers.Payroll_Management
{
    [Route("PayrollManagement/[controller]/[action]")]
    public class ViewEmployeesRoleContoller : Controller
    {
        private readonly IConfiguration _config;

        public ViewEmployeesRoleContoller(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        
        [HttpGet]
        public JsonResult GetAllRoles()
        {
            var roles = new List<string>();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT DISTINCT Role FROM Employees", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(reader["Role"].ToString());
                }
            }
            return Json(roles);
        }

        
        [HttpGet]
        public JsonResult GetEmployeesByRole(string role)
        {
            var list = new List<object>();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT EmployeeID, FirstName, LastName FROM Employees WHERE Role = @Role", con);
                cmd.Parameters.AddWithValue("@Role", role);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new
                    {
                        EmployeeID = reader["EmployeeID"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString()
                    });
                }
            }
            return Json(list);
        }
    }
}

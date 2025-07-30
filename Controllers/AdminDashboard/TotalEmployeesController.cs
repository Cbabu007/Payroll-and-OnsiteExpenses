using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace PayrollandOnsiteExpenses.Controllers.AdminDashboard
{
    [ApiController]
    [Route("api/employees")]
    public class TotalEmployees : Controller
    {
        private readonly IConfiguration _config;

        public TotalEmployees(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("rolesummary")]
        public IActionResult GetEmployeeRoleSummary()
        {
            var roleSummary = new List<object>();

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Role, COUNT(*) AS Total FROM Employees GROUP BY Role", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roleSummary.Add(new
                    {
                        role = reader["Role"].ToString(),
                        total = Convert.ToInt32(reader["Total"])
                    });
                }
            }

            return Ok(roleSummary);
        }
    }
}





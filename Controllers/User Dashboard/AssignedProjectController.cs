using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using PayrollandOnsiteExpenses.Models;

namespace PayrollandOnsiteExpenses.Controllers.User_Dashboard
{
    public class AssignedProjectController : Controller
    {
        private readonly IConfiguration _config;

        public AssignedProjectController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public JsonResult GetAssignedProjectsByEmployee()
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId"); 
            var list = new List<AssignedProjectModel>();

            if (string.IsNullOrEmpty(employeeId))
                return Json(new { success = false, message = "Session missing." });

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand(@"
                    SELECT 
                        pt.ProjectName, pt.StartDate, pt.EndDate,
                        pt.No, pt.Address1, pt.Address2, pt.Location,
                        pt.Taluk, pt.District, pt.Pincode
                    FROM AssignedProjectEmployees ape
                    INNER JOIN ProjectTable pt ON ape.ProjectName = pt.ProjectName
                    WHERE ape.EmployeeID = @EmployeeID", con);

                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new AssignedProjectModel
                    {
                        ProjectName = reader["ProjectName"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        No = reader["No"].ToString(),
                        Address1 = reader["Address1"].ToString(),
                        Address2 = reader["Address2"].ToString(),
                        Location = reader["Location"].ToString(),
                        Taluk = reader["Taluk"].ToString(),
                        District = reader["District"].ToString(),
                        Pincode = reader["Pincode"].ToString()
                    });
                }
            }

            return Json(list);
        }
    }
}

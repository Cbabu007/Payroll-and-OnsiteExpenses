using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollandOnsiteExpenses.Models;
using System;
using System.Collections.Generic;

namespace PayrollandOnsiteExpenses.Controllers.ProjectManagement
{
    [Route("ProjectManagement/[action]")]
    public class ProjectsController : Controller
    {
        private readonly IConfiguration _config;

        public ProjectsController(IConfiguration config)
        {
            _config = config;
        }

        
        [HttpPost]
        public IActionResult AddProject([FromBody] ProjectTable project)
        {
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var query = @"INSERT INTO ProjectTable 
                              (ProjectName, Location, StartDate, EndDate, No, Address1, Address2, Taluk, District, Pincode)
                              VALUES 
                              (@ProjectName, @Location, @StartDate, @EndDate, @No, @Address1, @Address2, @Taluk, @District, @Pincode)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                cmd.Parameters.AddWithValue("@Location", project.Location);
                cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", project.EndDate);
                cmd.Parameters.AddWithValue("@No", project.No ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address1", project.Address1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address2", project.Address2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Taluk", project.Taluk ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@District", project.District ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Pincode", project.Pincode ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }

        
        [HttpPost]
        public IActionResult UpdateProject([FromBody] ProjectTable project)
        {
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var query = @"UPDATE ProjectTable SET 
                                Location=@Location,
                                StartDate=@StartDate,
                                EndDate=@EndDate,
                                No=@No,
                                Address1=@Address1,
                                Address2=@Address2,
                                Taluk=@Taluk,
                                District=@District,
                                Pincode=@Pincode
                              WHERE ProjectName=@ProjectName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                cmd.Parameters.AddWithValue("@Location", project.Location);
                cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", project.EndDate);
                cmd.Parameters.AddWithValue("@No", project.No ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address1", project.Address1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address2", project.Address2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Taluk", project.Taluk ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@District", project.District ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Pincode", project.Pincode ?? (object)DBNull.Value);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Project not found!" });
            }
        }

        
        [HttpGet]
        public IActionResult ViewProjects()
        {
            List<ProjectTable> projects = new List<ProjectTable>();

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string query = "SELECT * FROM ProjectTable";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    projects.Add(new ProjectTable
                    {
                        ProjectName = reader["ProjectName"].ToString(),
                        Location = reader["Location"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        No = reader["No"].ToString(),
                        Address1 = reader["Address1"].ToString(),
                        Address2 = reader["Address2"].ToString(),
                        Taluk = reader["Taluk"].ToString(),
                        District = reader["District"].ToString(),
                        Pincode = reader["Pincode"].ToString()
                    });
                }
            }

            return View(projects);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollandOnsiteExpenses.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace PayrollandOnsiteExpenses.Controllers.Claim_StatusViewer_User
{
    public class CurrentProjectStatusController : Controller
    {
        private readonly IConfiguration _config;

        public CurrentProjectStatusController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString() => _config.GetConnectionString("DefaultConnection");

        [HttpGet]
        public JsonResult GetProjectStatusByEmployee()
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId"); 
            var list = new List<CurrentProjectStatusModel>();

            if (string.IsNullOrEmpty(employeeId))
                return Json(new { success = false, message = "Session missing." });

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetCurrentProjectStatusByEmployee", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new CurrentProjectStatusModel
                        {
                            ProjectName = reader["ProjectName"].ToString(),
                            Travel = reader["TravelStatus"].ToString(),
                            Food = reader["FoodStatus"].ToString(),
                            Accommodation = reader["AccommodationStatus"].ToString(),
                             Status = reader["Status"].ToString() 
                        });
                    }
                }
            }

            return Json(list);
        }
    }
}

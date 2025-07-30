using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollandOnsiteExpenses.Models;
using System.Collections.Generic;

namespace PayrollandOnsiteExpenses.Controllers.AdminDashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingApprovalsController : Controller
    {
        private readonly IConfiguration _config;

        public PendingApprovalsController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        
        [HttpGet("approved-list")]
        public IActionResult GetApprovedList()
        {
            var list = new List<PendingApprovalsModel>();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                string query = @"
            SELECT EmployeeID, ProjectName,
                   ISNULL(TravelAmount, 0) + ISNULL(FoodAmount, 0) + ISNULL(AccommodationAmount, 0) AS TotalAmount
            FROM ExpenseApprovalSummary
            WHERE TravelStatus = 'Ok' AND FoodStatus = 'Ok' AND AccommodationStatus = 'Ok'
        ";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new PendingApprovalsModel
                    {
                        EmployeeID = reader["EmployeeID"].ToString(),
                        ProjectName = reader["ProjectName"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        Status = "Pending" 
                    });
                }
            }

            return Ok(list); 
        }


        
        [HttpPost("submit")]
        public IActionResult SubmitApproval([FromBody] PendingApprovalsModel model)
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

                string insertQuery = @"
                    INSERT INTO PendingApprovals (EmployeeID, ProjectName, TotalAmount, Status)
                    VALUES (@EmployeeID, @ProjectName, @TotalAmount, @Status)
                ";

                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                cmd.Parameters.AddWithValue("@ProjectName", model.ProjectName);
                cmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                cmd.Parameters.AddWithValue("@Status", model.Status);
                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Approval submitted successfully." });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollandOnsiteExpenses.Models;
using System.Data;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    [ApiController]
    [Route("ReportsPage/[controller]")]
    public class FetchEmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public FetchEmployeeController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString() => _config.GetConnectionString("DefaultConnection");

        
        [HttpGet("GetAllEmployeeIDs")]
        public IActionResult GetAllEmployeeIDs()
        {
            var list = new List<object>();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT EmployeeID FROM Employees", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new
                    {
                        employeeID = reader["EmployeeID"].ToString()
                    });
                }
            }
            return Json(list);
        }

        
        [HttpGet("GetPayslipDetails")]
        public IActionResult GetPayslipDetails(string employeeID, string month, string year)
        {
            var model = new FetchModel();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                
                string query1 = @"SELECT TOP 1 * FROM MonthlyPayslip 
                                  WHERE EmployeeID = @EmployeeID AND Month = @Month AND Year = @Year";
                SqlCommand cmd1 = new SqlCommand(query1, con);
                cmd1.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd1.Parameters.AddWithValue("@Month", month);
                cmd1.Parameters.AddWithValue("@Year", year);

                using (SqlDataReader reader = cmd1.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.EmployeeID = employeeID;
                        model.EmployeeName = reader["EmployeeName"].ToString();
                        model.Designation = reader["Designation"].ToString();
                        model.LOPDays = reader["LOPDays"].ToString();
                        model.BasicSalary = reader["BasicSalary"].ToString();
                        model.HRA = reader["HRA"].ToString();
                        model.ConveyanceAllowance = reader["ConveyanceAllowance"].ToString();
                        model.GrossEarnings = reader["GrossEarnings"].ToString();
                        model.EPFContribution = reader["EPFContribution"].ToString();
                        model.HealthInsurance = reader["HealthInsurance"].ToString();
                        model.TotalDeductions = reader["TotalDeductions"].ToString(); 

                        model.TotalNetPayable = reader["TotalNetPayable"].ToString();
                    }
                }

                
                string query2 = @"SELECT TOP 1 * FROM PFRegistrations WHERE EmployeeID = @EmployeeID";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.Parameters.AddWithValue("@EmployeeID", employeeID);
                using (SqlDataReader reader2 = cmd2.ExecuteReader())
                {
                    if (reader2.Read())
                    {
                        model.DateOfJoining = reader2["DateOfJoining"].ToString();
                        model.PFAccountNumber = reader2["PFAcNumber"].ToString();
                        model.UAN = reader2["UAN"].ToString();
                    }
                }
            }

            return Json(model);
        }
    }
}

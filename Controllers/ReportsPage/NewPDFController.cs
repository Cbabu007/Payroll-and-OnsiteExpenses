
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollandOnsiteExpenses.Models;
using System.Data;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    [ApiController]
    [Route("ReportsPage/[controller]")]
    public class NewPDFController : Controller
    {
        private readonly IConfiguration _config;

        public NewPDFController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString() => _config.GetConnectionString("DefaultConnection");

        [HttpGet("GetPayslipData")]
        public IActionResult GetPayslipData(string employeeID, string month, int year)
        {
            var result = new PDFModel();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

               
                using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM MonthlyPayslip 
                    WHERE EmployeeID = @EmployeeID AND Month = @Month AND Year = @Year", con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.EmployeeID = reader["EmployeeID"].ToString();
                            result.EmployeeName = reader["EmployeeName"].ToString();
                            result.Designation = reader["Designation"].ToString();
                            result.Month = reader["Month"].ToString();
                            result.Year = Convert.ToInt32(reader["Year"]);
                            result.LOPDays = Convert.ToInt32(reader["LOPDays"]);
                            result.BasicSalary = Convert.ToDecimal(reader["BasicSalary"]);
                            result.HouseRentAllowance = Convert.ToDecimal(reader["HouseRentAllowance"]);
                            result.ConveyanceAllowance = Convert.ToDecimal(reader["ConveyanceAllowance"]);
                            result.GrossEarnings = Convert.ToDecimal(reader["GrossEarnings"]);
                            result.EPFContribution = Convert.ToDecimal(reader["EPFContribution"]);
                            result.HealthInsurance = Convert.ToDecimal(reader["HealthInsurance"]);
                            result.TotalDeducation = Convert.ToDecimal(reader["TotalDeducation"]);
                            result.TotalNetPayable = Convert.ToDecimal(reader["TotalNetPayable"]);
                        }
                    }
                }

                
                using (SqlCommand cmd2 = new SqlCommand(@"SELECT DateOfJoining, PFAcNumber, UAN 
                    FROM PFRegistrations WHERE EmployeeID = @EmployeeID", con))
                {
                    cmd2.Parameters.AddWithValue("@EmployeeID", employeeID);

                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.DateofJoining = Convert.ToDateTime(reader["DateOfJoining"]);
                            result.PFAcNumber = reader["PFAcNumber"].ToString();
                            result.UAN = reader["UAN"].ToString();
                        }
                    }
                }
            }

            return Ok(result);
        }
    }
}

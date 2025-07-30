using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PayrollandOnsiteExpenses.Models;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    public class TableGenerateController : Controller
    {
        private readonly IConfiguration _configuration;

        public TableGenerateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public JsonResult GetEmployeeDropdown()
        {
            List<object> employees = new List<object>();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                string query = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM Employees";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    employees.Add(new
                    {
                        employeeID = rdr["EmployeeID"].ToString(),
                        fullName = rdr["FullName"].ToString()
                    });
                }
            }

            return Json(employees);
        }

        [HttpGet]
        public JsonResult GetEmployeeFullDetails(string employeeID)
        {
            var model = new TableGenerateModel();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

                
                using (SqlCommand cmd = new SqlCommand("SELECT FirstName + ' ' + LastName AS Name, Role FROM Employees WHERE EmployeeID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", employeeID);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            model.EmployeeName = rdr["Name"].ToString();
                            model.Designation = rdr["Role"].ToString();
                        }
                    }
                }

               
                using (SqlCommand cmd = new SqlCommand("SELECT DateOfJoining, PFAcNumber, UAN FROM PFRegistrations WHERE EmployeeID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", employeeID);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            model.DateOfJoining = Convert.ToDateTime(rdr["DateOfJoining"]);
                            model.PFAcNumber = rdr["PFAcNumber"].ToString();
                            model.UAN = rdr["UAN"].ToString();
                        }
                    }
                }

                
                using (SqlCommand cmd = new SqlCommand("SELECT BasicSalary, HRA, DeductionPercentage FROM PayrollTable WHERE EmployeeID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", employeeID);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            model.BasicSalary = Convert.ToDecimal(rdr["BasicSalary"]);
                            model.HouseRentAllowance = Convert.ToDecimal(rdr["HRA"]);
                            model.LOPDays = Convert.ToInt32(rdr["DeductionPercentage"]);
                        }
                    }
                }

                
                using (SqlCommand cmd = new SqlCommand(@"SELECT 
                        ISNULL(SUM(FoodAmount), 0) AS Food,
                        ISNULL(SUM(TravelAmount), 0) AS Travel,
                        ISNULL(SUM(AccommodationAmount), 0) AS Accommodation
                    FROM ExpenseApprovalSummary WHERE EmployeeID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", employeeID);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            model.Food = Convert.ToDecimal(rdr["Food"]);
                            model.Travel = Convert.ToDecimal(rdr["Travel"]);
                            model.Accommodation = Convert.ToDecimal(rdr["Accommodation"]);
                        }
                    }
                }
            }

            return Json(model);
        }
    }
}

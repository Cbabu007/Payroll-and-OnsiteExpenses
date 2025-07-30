using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PayrollandOnsiteExpenses.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace PayrollandOnsiteExpenses.Controllers.EmployeeSalaryDetails
{
    public class EmployeeSalaryDetailsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public EmployeeSalaryDetailsController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        private string GetConnectionString() => _config.GetConnectionString("DefaultConnection");



        [HttpGet]
        [Route("/EmployeeSalaryDetails/GetAllEmployeeIDs")]
        public JsonResult GetAllEmployeeIDs()
        {
            var list = new List<object>();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT EmployeeID FROM Employees", con);
                var reader = cmd.ExecuteReader();
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

        
        [HttpPost]
        public IActionResult Submit(EmployeeSalaryDetailsModel model)
        {
            string filePath = null;

            if (model.Payslip != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Payslip.FileName);
                var savePath = Path.Combine(_env.WebRootPath, "uploads", "payslips");

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                var fullPath = Path.Combine(savePath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    model.Payslip.CopyTo(stream);
                }

                filePath = "/uploads/payslips/" + fileName;
            }

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                string query = @"
                    INSERT INTO MonthlyPayslip
                    (EmployeeID, EmployeeName, Designation, Month, Year, LOPDays,
                     BasicSalary, HRA, Food, Travel, Accommodation, ConveyanceAllowance, GrossEarnings,
                     EPFContribution, HealthInsurance, TotalDeductions, TotalNetPayable, PayslipFilePath)
                    VALUES
                    (@EmployeeID, @EmployeeName, @Designation, @Month, @Year, @LOPDays,
                     @BasicSalary, @HRA, @Food, @Travel, @Accommodation, @ConveyanceAllowance, @GrossEarnings,
                     @EPFContribution, @HealthInsurance, @TotalDeductions, @TotalNetPayable, @PayslipFilePath)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                    cmd.Parameters.AddWithValue("@EmployeeName", model.EmployeeName);
                    cmd.Parameters.AddWithValue("@Designation", model.Designation);
                    cmd.Parameters.AddWithValue("@Month", model.Month);
                    cmd.Parameters.AddWithValue("@Year", model.Year);

                    cmd.Parameters.AddWithValue("@LOPDays", model.LOPDays);
                    cmd.Parameters.AddWithValue("@BasicSalary", model.BasicSalary);
                    cmd.Parameters.AddWithValue("@HRA", model.HRA);
                    cmd.Parameters.AddWithValue("@Food", model.Food);
                    cmd.Parameters.AddWithValue("@Travel", model.Travel);
                    cmd.Parameters.AddWithValue("@Accommodation", model.Accommodation);
                    cmd.Parameters.AddWithValue("@ConveyanceAllowance", model.ConveyanceAllowance);
                    cmd.Parameters.AddWithValue("@GrossEarnings", model.GrossEarnings);
                    cmd.Parameters.AddWithValue("@EPFContribution", model.EPFContribution);
                    cmd.Parameters.AddWithValue("@HealthInsurance", model.HealthInsurance);
                    cmd.Parameters.AddWithValue("@TotalDeductions", model.TotalDeductions);
                    cmd.Parameters.AddWithValue("@TotalNetPayable", model.TotalNetPayable);
                    cmd.Parameters.AddWithValue("@PayslipFilePath", (object)filePath ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            return Json(new { success = true, message = "Payslip submitted successfully!" });
        }

       
        [HttpGet]
        [Route("/EmployeeSalaryDetails/GetEmployeeDetails")]
        public JsonResult GetEmployeeDetails(string employeeId)
        {
            var model = new EmployeeSalaryDetailsModel();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

                var empCmd = new SqlCommand("SELECT EmployeeID, FirstName, LastName, Role FROM Employees WHERE LTRIM(RTRIM(EmployeeID)) = @id", con);
                empCmd.Parameters.AddWithValue("@id", employeeId);
                var reader = empCmd.ExecuteReader();
                if (reader.Read())
                {
                    model.EmployeeID = reader["EmployeeID"].ToString();
                    model.EmployeeName = reader["FirstName"] + " " + reader["LastName"];
                    model.Designation = reader["Role"].ToString();
                }
                reader.Close();



                var payrollCmd = new SqlCommand("SELECT BasicSalary, HRA FROM PayrollTable WHERE LTRIM(RTRIM(EmployeeID)) = @id", con);
                payrollCmd.Parameters.AddWithValue("@id", employeeId);
                reader = payrollCmd.ExecuteReader();
                if (reader.Read())
                {
                    model.BasicSalary = Convert.ToDecimal(reader["BasicSalary"]);
                    model.HRA = Convert.ToDecimal(reader["HRA"]);
                }
                reader.Close();

                var expCmd = new SqlCommand("SELECT TravelAmount, FoodAmount, AccommodationAmount FROM ExpenseApprovalSummary WHERE LTRIM(RTRIM(EmployeeID)) = @id", con);
                expCmd.Parameters.AddWithValue("@id", employeeId);
                reader = expCmd.ExecuteReader();
                if (reader.Read())
                {
                    model.Travel = Convert.ToDecimal(reader["TravelAmount"]);
                    model.Food = Convert.ToDecimal(reader["FoodAmount"]);
                    model.Accommodation = Convert.ToDecimal(reader["AccommodationAmount"]);
                }
                reader.Close();
            }

            return Json(model);
        }
        [HttpGet]
        [Route("/EmployeeSalaryDetails/GetPayrollDetails")]
        public JsonResult GetPayrollDetails(string employeeId, string month, int year)
        {
            var data = new
            {
                lopDays = 0,
                basicSalary = 0m,
                hra = 0m,
                food = 0m,
                travel = 0m,
                accommodation = 0m
            };

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

                
                var cmd = new SqlCommand(@"SELECT DeductionPercentage, BasicSalary, HRA 
                                   FROM PayrollTable 
                                   WHERE LTRIM(RTRIM(EmployeeID)) = @id AND Month = @month AND Year = @year", con);
                cmd.Parameters.AddWithValue("@id", employeeId);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    data = new
                    {
                        lopDays = Convert.ToInt32(reader["DeductionPercentage"]),
                        basicSalary = Convert.ToDecimal(reader["BasicSalary"]),
                        hra = Convert.ToDecimal(reader["HRA"]),
                        food = 0m,
                        travel = 0m,
                        accommodation = 0m
                    };
                }
                reader.Close();

               
                var expCmd = new SqlCommand(@"SELECT TravelAmount, FoodAmount, AccommodationAmount 
                                      FROM ExpenseApprovalSummary 
                                      WHERE LTRIM(RTRIM(EmployeeID)) = @id", con);
                expCmd.Parameters.AddWithValue("@id", employeeId);
                reader = expCmd.ExecuteReader();
                if (reader.Read())
                {
                    data = new
                    {
                        lopDays = data.lopDays,
                        basicSalary = data.basicSalary,
                        hra = data.hra,
                        food = Convert.ToDecimal(reader["FoodAmount"]),
                        travel = Convert.ToDecimal(reader["TravelAmount"]),
                        accommodation = Convert.ToDecimal(reader["AccommodationAmount"])
                    };
                }
                reader.Close();
            }

            return Json(data);
        }

        [HttpGet]
        [Route("/EmployeeSalaryDetails/GetAllPayslips")]
        public JsonResult GetAllPayslips()
        {
            var list = new List<object>();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM MonthlyPayslip", con);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new
                    {
                        EmployeeID = reader["EmployeeID"].ToString(),
                        EmployeeName = reader["EmployeeName"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        LOPDays = reader["LOPDays"].ToString(),
                        BasicSalary = reader["BasicSalary"].ToString(),
                        HRA = reader["HRA"].ToString(),
                        Food = reader["Food"].ToString(),
                        Travel = reader["Travel"].ToString(),
                        Accommodation = reader["Accommodation"].ToString(),
                        ConveyanceAllowance = reader["ConveyanceAllowance"].ToString(),
                        GrossEarnings = reader["GrossEarnings"].ToString(),
                        EPFContribution = reader["EPFContribution"].ToString(),
                        HealthInsurance = reader["HealthInsurance"].ToString(),
                        TotalDeductions = reader["TotalDeductions"].ToString(),
                        TotalNetPayable = reader["TotalNetPayable"].ToString(),
                        PayslipFilePath = reader["PayslipFilePath"].ToString()
                    });
                }

                reader.Close();
            }

            return Json(list);
        }

    }
}
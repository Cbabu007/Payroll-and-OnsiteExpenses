using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using PayrollandOnsiteExpenses.Models;
using Microsoft.AspNetCore.Http;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    public class PayslipGenerateController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public PayslipGenerateController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        
        [HttpGet]
        public JsonResult GetAllEmployees()
        {
            List<dynamic> employees = new List<dynamic>();
            string query = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM Employees";

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
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
        public JsonResult GetEmployeeDetails(string employeeID)
        {
            var model = new
            {
                employeeName = "",
                designation = "",
                dateofJoining = (DateTime?)null,
                lopDays = 0,
                pfAcNumber = "",
                uan = "",
                basicSalary = 0.0m,
                houseRentAllowance = 0.0m,
                conveyanceAllowance = 0.0m,
                grossEarnings = 0.0m,
                epfContribution = 0.0m
            };

            string empName = "", designation = "", pfNumber = "", uan = "";
            DateTime? doj = null;
            int lop = 0;
            decimal basic = 0, hra = 0, conveyance = 0, epf = 0;

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

               
                string empQuery = "SELECT FirstName + ' ' + LastName AS Name, Role FROM Employees WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(empQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        empName = rdr["Name"].ToString();
                        designation = rdr["Role"].ToString();
                    }
                    rdr.Close();
                }

                
                string pfQuery = "SELECT DateOfJoining, DeductionPercentage, PFAcNumber, UAN FROM PFRegistrations WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(pfQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        doj = Convert.ToDateTime(rdr["DateOfJoining"]);
                        lop = Convert.ToInt32(rdr["DeductionPercentage"]);
                        pfNumber = rdr["PFAcNumber"].ToString();
                        uan = rdr["UAN"].ToString();
                    }
                    rdr.Close();
                }

                
                string payrollQuery = "SELECT BasicSalary, HRA FROM PayrollTable WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(payrollQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        basic = Convert.ToDecimal(rdr["BasicSalary"]);
                        hra = Convert.ToDecimal(rdr["HRA"]);
                        epf = Math.Round(basic * 0.12M, 2);
                    }
                    rdr.Close();
                }

               
                string expenseQuery = "SELECT ISNULL(SUM(TotalAmount),0) AS Conveyance FROM ExpenseApprovalSummary WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(expenseQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        conveyance = Convert.ToDecimal(rdr["Conveyance"]);
                    }
                    rdr.Close();
                }
            }

            decimal gross = basic + hra + conveyance;

           
            return Json(new
            {
                employeeName = empName,
                designation = designation,
                dateofJoining = doj,
                lopDays = lop,
                pfAcNumber = pfNumber,
                uan = uan,
                basicSalary = basic,
                houseRentAllowance = hra,
                conveyanceAllowance = conveyance,
                grossEarnings = gross,
                epfContribution = epf
            });
        }

       
        [HttpPost]
        public async Task<IActionResult> SubmitPayslip(IFormCollection form, IFormFile IssuedBy)
        {
            string filePath = null;

            
            if (IssuedBy != null && IssuedBy.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/signatures");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(IssuedBy.FileName);
                filePath = Path.Combine("uploads/signatures", uniqueFileName);
                string fullPath = Path.Combine(_environment.WebRootPath, filePath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await IssuedBy.CopyToAsync(stream);
                }
            }

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string query = @"INSERT INTO PayslipGenerate 
        (EmployeeID, EmployeeName, Designation, PayPeriod, DateofJoining, PayDate, PaidDays, LOPDays, PFAcNumber, UAN,
         BasicSalary, HouseRentAllowance, ConveyanceAllowance, GrossEarnings, EPFContribution, HealthInsurance, 
         TotalDeducation, TotalNetPayable, IssuedBy)
        VALUES
        (@EmployeeID, @EmployeeName, @Designation, @PayPeriod, @DateofJoining, @PayDate, @PaidDays, @LOPDays, 
         @PFAcNumber, @UAN, @BasicSalary, @HouseRentAllowance, @ConveyanceAllowance, @GrossEarnings, @EPFContribution, 
         @HealthInsurance, @TotalDeducation, @TotalNetPayable, @IssuedBy)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@EmployeeID", form["EmployeeID"]);
                cmd.Parameters.AddWithValue("@EmployeeName", form["EmployeeName"]);
                cmd.Parameters.AddWithValue("@Designation", form["Designation"]);
                cmd.Parameters.AddWithValue("@PayPeriod", form["PayPeriod"]);
                cmd.Parameters.AddWithValue("@DateofJoining", DateTime.Parse(form["DateofJoining"]));
                cmd.Parameters.AddWithValue("@PayDate", DateTime.Parse(form["PayDate"]));
                cmd.Parameters.AddWithValue("@PaidDays", int.Parse(form["PaidDays"]));
                cmd.Parameters.AddWithValue("@LOPDays", int.Parse(form["LOPDays"]));
                cmd.Parameters.AddWithValue("@PFAcNumber", form["PFAcNumber"]);
                cmd.Parameters.AddWithValue("@UAN", form["UAN"]);
                cmd.Parameters.AddWithValue("@BasicSalary", decimal.Parse(form["BasicSalary"]));
                cmd.Parameters.AddWithValue("@HouseRentAllowance", decimal.Parse(form["HouseRentAllowance"]));
                cmd.Parameters.AddWithValue("@ConveyanceAllowance", decimal.Parse(form["ConveyanceAllowance"]));
                cmd.Parameters.AddWithValue("@GrossEarnings", decimal.Parse(form["GrossEarnings"]));
                cmd.Parameters.AddWithValue("@EPFContribution", decimal.Parse(form["EPFContribution"]));
                cmd.Parameters.AddWithValue("@HealthInsurance", decimal.Parse(form["HealthInsurance"]));
                cmd.Parameters.AddWithValue("@TotalDeducation", decimal.Parse(form["TotalDeducation"]));
                cmd.Parameters.AddWithValue("@TotalNetPayable", decimal.Parse(form["TotalNetPayable"]));
                cmd.Parameters.AddWithValue("@IssuedBy", filePath ?? string.Empty);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return Json(new { success = true, message = "Payslip generated and saved successfully!" });
        }
    }
}

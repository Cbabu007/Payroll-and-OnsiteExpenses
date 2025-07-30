using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollandOnsiteExpenses.Models.ReportsPage;
using System.Data;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    [ApiController]
    public class USPDFGenerteControllers : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public USPDFGenerteControllers(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

       
        [HttpPost("/USPDFGenerteControllers/SavePayslip")]
        public async Task<IActionResult> SavePayslip()
        {
            try
            {
                var form = Request.Form;

                var employeeId = form["EmployeeID"];
                var payslipMonth = form["PayslipMonth"];
                var employeeName = form["EmployeeName"];
                var designation = form["Designation"];
                var dateOfJoining = form["DateOfJoining"];
                var payPeriod = form["PayPeriod"];
                var payDate = form["PayDate"];
                var paidDays = form["PaidDays"];
                var lopDays = form["LOPDays"];
                var pfAc = form["PFAcNumber"];
                var uan = form["UAN"];
                var basic = form["Basic"];
                var hra = form["HRA"];
                var ca = form["ConveyanceAllowance"];
                var epf = form["EPFContribution"];
                var health = form["HealthContribution"];
                var gross = form["GrossEarnings"];
                var deductions = form["TotalDeductions"];
                var netPay = form["TotalNetPayable"];

                
                var file = form.Files["IssuedByFile"];
                string fileName = "";

                if (file != null && file.Length > 0)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/signatures");
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    return BadRequest(new { message = "No file was uploaded." });
                }

                
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    string insertQuery = @"
                    INSERT INTO PayslipRecords
                    (EmployeeID, EmployeeName, Designation, DateOfJoining, PayslipMonth, PayPeriod, PayDate,
                     PaidDays, LOPDays, PFAcNumber, UAN, Basic, HRA, ConveyanceAllowance, EPFContribution,
                     HealthContribution, GrossEarnings, TotalDeductions, TotalNetPayable, SignatureFileName)
                    VALUES
                    (@EmployeeID, @EmployeeName, @Designation, @DateOfJoining, @PayslipMonth, @PayPeriod, @PayDate,
                     @PaidDays, @LOPDays, @PFAcNumber, @UAN, @Basic, @HRA, @CA, @EPF, @Health, @Gross, @Deductions, @NetPay, @Signature)";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                    cmd.Parameters.AddWithValue("@Designation", designation);
                    cmd.Parameters.AddWithValue("@DateOfJoining", dateOfJoining);
                    cmd.Parameters.AddWithValue("@PayslipMonth", payslipMonth);
                    cmd.Parameters.AddWithValue("@PayPeriod", payPeriod);
                    cmd.Parameters.AddWithValue("@PayDate", payDate);
                    cmd.Parameters.AddWithValue("@PaidDays", paidDays);
                    cmd.Parameters.AddWithValue("@LOPDays", lopDays);
                    cmd.Parameters.AddWithValue("@PFAcNumber", pfAc);
                    cmd.Parameters.AddWithValue("@UAN", uan);
                    cmd.Parameters.AddWithValue("@Basic", basic);
                    cmd.Parameters.AddWithValue("@HRA", hra);
                    cmd.Parameters.AddWithValue("@CA", ca);
                    cmd.Parameters.AddWithValue("@EPF", epf);
                    cmd.Parameters.AddWithValue("@Health", health);
                    cmd.Parameters.AddWithValue("@Gross", gross);
                    cmd.Parameters.AddWithValue("@Deductions", deductions);
                    cmd.Parameters.AddWithValue("@NetPay", netPay);
                    cmd.Parameters.AddWithValue("@Signature", fileName);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine($"Payslip saved for {employeeName}, File: {fileName}");

                return Json(new { message = "Payslip saved successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving payslip: " + ex.Message);
                return StatusCode(500, new { message = "Error saving payslip", error = ex.Message });
            }
        }

        
        [HttpGet("/api/Employees/GetEmployeeIDs")]
        public async Task<IActionResult> GetEmployeeIDs()
        {
            List<string> employeeIds = new List<string>();

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT EmployeeID FROM Employees";
                SqlCommand cmd = new SqlCommand(query, conn);
                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employeeIds.Add(reader["EmployeeID"].ToString());
                }
            }

            return Ok(employeeIds);
        }

       
        [HttpGet("/api/Employees/GetEmployeeDetails/{id}")]
        public async Task<IActionResult> GetEmployeeDetails(string id)
        {
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                string query = @"
                SELECT 
                    e.FirstName + ' ' + e.LastName AS EmployeeName,
                    e.Role,
                    pr.DateOfJoining,
                    pr.PFAcNumber,
                    pr.UAN,
                    p.BasicSalary,
                    p.HRA,
                    ISNULL(es.TravelAmount,0) + ISNULL(es.FoodAmount,0) + ISNULL(es.AccommodationAmount,0) AS Allowance,
                    p.DeductionPercentage AS LOP
                FROM Employees e
                LEFT JOIN PFRegistrations pr ON e.EmployeeID = pr.EmployeeID
                LEFT JOIN PayrollTable p ON e.EmployeeID = p.EmployeeID
                LEFT JOIN ExpenseApprovalSummary es ON e.EmployeeID = es.EmployeeId
                WHERE e.EmployeeID = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var data = new
                    {
                        employeeName = reader["EmployeeName"].ToString(),
                        role = reader["Role"].ToString(),
                        dateOfJoining = reader["DateOfJoining"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfJoining"]).ToString("yyyy-MM-dd") : "",
                        pfAccount = reader["PFAcNumber"]?.ToString() ?? "",
                        uan = reader["UAN"]?.ToString() ?? "",
                        basic = reader["BasicSalary"] != DBNull.Value ? Convert.ToDecimal(reader["BasicSalary"]) : 0,
                        hra = reader["HRA"] != DBNull.Value ? Convert.ToDecimal(reader["HRA"]) : 0,
                        allowance = reader["Allowance"] != DBNull.Value ? Convert.ToDecimal(reader["Allowance"]) : 0,
                        lop = reader["LOP"] != DBNull.Value ? Convert.ToInt32(reader["LOP"]) : 0
                    };

                    return Ok(data);
                }
            }

            return NotFound(new { message = "Employee details not found." });
        }
    }
}

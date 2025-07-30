using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using PayrollandOnsiteExpenses.Models;

[Route("api/[controller]")]
[ApiController]
public class ViewPayrollDataController : ControllerBase
{
    private readonly string connectionString = "Server=DESKTOP-3LPFR9G\\SQLEXPRESS;Database=Payroll;Trusted_Connection=True;TrustServerCertificate=True;";

    
    [HttpGet("employees")]
    public IActionResult GetEmployeeNames()
    {
        var employeeNames = new List<string>();
        using (var con = new SqlConnection(connectionString))
        {
            string query = "SELECT DISTINCT EmployeeName FROM PayrollTable WHERE EmployeeName IS NOT NULL";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                employeeNames.Add(rdr["EmployeeName"].ToString());
            }
        }
        return Ok(employeeNames);
    }

    
    [HttpGet("payroll/{employeeName}")]
    public IActionResult GetPayrollByEmployee(string employeeName)
    {
        var records = new List<PayrollModel>();
        using (var con = new SqlConnection(connectionString))
        {
            string query = @"
    SELECT EmployeeID, EmployeeName, Month, Year, BasicSalary, HRA, Allowances, 
           Deductions, NetSalary, GeneratedDate 
    FROM PayrollTable 
    WHERE EmployeeName = @EmployeeName";


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                records.Add(new PayrollModel
                {
                    EmployeeID = rdr["EmployeeID"].ToString(),
                    EmployeeName = rdr["EmployeeName"].ToString(),
                    Month = rdr["Month"].ToString(),
                    Year = Convert.ToInt32(rdr["Year"]),
                    BasicSalary = Convert.ToDecimal(rdr["BasicSalary"]),
                    HRA = Convert.ToDecimal(rdr["HRA"]),
                    Allowances = Convert.ToDecimal(rdr["Allowances"]),
                    Deductions = Convert.ToDecimal(rdr["Deductions"]),
                    NetSalary = Convert.ToDecimal(rdr["NetSalary"]),
                    GeneratedDate = Convert.ToDateTime(rdr["GeneratedDate"])
                });
            }
        }
        return Ok(records);
    }

    [HttpDelete("delete/{employeeID}/{generatedDate}")]
    public IActionResult DeletePayslip(string employeeID, DateTime generatedDate)
    {
        using (var con = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM PayrollTable WHERE EmployeeID = @EmployeeID AND GeneratedDate = @GeneratedDate";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@GeneratedDate", generatedDate);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? Ok("Deleted") : NotFound("No record found.");
            }
        }
    }


}

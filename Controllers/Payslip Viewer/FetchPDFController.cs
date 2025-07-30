using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollandOnsiteExpenses.Models;
using System.Data;
using System.IO;

namespace PayrollandOnsiteExpenses.Controllers.Payslip_Viewer
{
    [Route("[controller]/[action]")]
    public class FetchPDFController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _uploadFolder;

        public FetchPDFController(IConfiguration config)
        {
            _config = config;
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "payslips");
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult GetEmployeeIDsFromUploads()
        {
            var ids = new List<string>();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT DISTINCT EmployeeID FROM UploadPayslip", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(reader["EmployeeID"].ToString());
                }
            }
            return Json(ids);
        }

        [HttpGet]
        public IActionResult DownloadPayslip(string EmployeeID, string Month, string Year)
        {
            string filePath = null;

            using (var con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                var cmd = new SqlCommand("GetPayslipFileByDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                cmd.Parameters.AddWithValue("@Month", Month);
                cmd.Parameters.AddWithValue("@Year", Year);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    filePath = reader["FilePath"].ToString();
                }
            }

            if (string.IsNullOrEmpty(filePath))
                return NotFound("Payslip not found.");

            var fullPath = Path.Combine(_uploadFolder, filePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found on server.");

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/pdf", Path.GetFileName(fullPath));
        }
    }
}

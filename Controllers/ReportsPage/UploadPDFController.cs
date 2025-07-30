using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollandOnsiteExpenses.Models;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace PayrollandOnsiteExpenses.Controllers.ReportsPage
{
    [Route("ReportsPage/[controller]/[action]")]
    public class UploadPDFController : Controller
    {
        private readonly IConfiguration _config;

        public UploadPDFController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPayslip([FromForm] UploadPayslipModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "payslips");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{model.EmployeeID}_{model.Month}_{model.Year}.pdf";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PdfFile.CopyToAsync(stream);
                }

                using (var con = new SqlConnection(GetConnectionString()))
                {
                    con.Open();
                    var cmd = new SqlCommand("InsertUploadPayslip", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                    cmd.Parameters.AddWithValue("@Month", model.Month);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@FilePath", fileName);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "PDF uploaded successfully!" });
            }

            return Json(new { success = false, message = "Upload failed. Check inputs." });
        }
    }
}

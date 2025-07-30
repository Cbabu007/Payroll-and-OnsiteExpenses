using Microsoft.AspNetCore.Http;

namespace PayrollandOnsiteExpenses.Models
{
    public class UploadPayslipModel
    {
        public string EmployeeID { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public IFormFile PdfFile { get; set; }
    }
}

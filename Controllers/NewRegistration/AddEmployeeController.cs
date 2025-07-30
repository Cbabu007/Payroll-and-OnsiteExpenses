using Microsoft.AspNetCore.Mvc;
using PayrollandOnsiteExpenses.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PayrollandOnsiteExpenses.Controllers.NewRegistration
{
    public class AddEmployeeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AddEmployeeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public IActionResult Create(NewRegistrationEmployee model)
        {
            if (!ModelState.IsValid) return View(model);

            string photoPath = null;
            if (model.Photo != null && model.Photo.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "photos");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = model.EmployeeID + "_" + Path.GetFileName(model.Photo.FileName);
                photoPath = Path.Combine("uploads/photos", uniqueFileName);
                using (var stream = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                {
                    model.Photo.CopyTo(stream);
                }
            }

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("InsertNewRegistration", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                cmd.Parameters.AddWithValue("@AltPhoneNumber", model.AltPhoneNumber);
                cmd.Parameters.AddWithValue("@DOB", model.DOB);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@DoorNo", model.DoorNo);
                cmd.Parameters.AddWithValue("@Address1", model.Address1);
                cmd.Parameters.AddWithValue("@Address2", model.Address2);
                cmd.Parameters.AddWithValue("@Location", model.Location);
                cmd.Parameters.AddWithValue("@Taluk", model.Taluk);
                cmd.Parameters.AddWithValue("@District", model.District);
                cmd.Parameters.AddWithValue("@State", model.State);
                cmd.Parameters.AddWithValue("@Pincode", model.Pincode);
                cmd.Parameters.AddWithValue("@Country", model.Country);
                cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                cmd.Parameters.AddWithValue("@EmployeeUsername", model.EmployeeUsername);
                cmd.Parameters.AddWithValue("@EmployeePassword", model.EmployeePassword);
                cmd.Parameters.AddWithValue("@Role", model.Role);

                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Success"] = "Employee registered successfully!";
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}

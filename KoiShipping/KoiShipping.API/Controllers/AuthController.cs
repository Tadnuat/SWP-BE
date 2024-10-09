using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using KoiShipping.API.Models.LoginModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using System.Linq;
using KoiShipping.API.Models.CustomerModel;

namespace KoiShipping.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork, TokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        [HttpPost("loginstaff")]
        public IActionResult LoginStaff([FromBody] LoginModelStaff loginModel)
        {
            // Kiểm tra trong bảng Staff (admin)
            var staff = _unitOfWork.StaffRepository.Get(x => x.Email == loginModel.Email && x.Password == loginModel.Password && x.DeleteStatus == false).FirstOrDefault();

            if (staff != null)
            {
                var token = _tokenService.GenerateToken(staff.Email, staff.Role);
                return Ok(new { token, id = staff.StaffId, name = staff.StaffName, Role = staff.Role });
            }

            return Unauthorized("Invalid email or password.");
        }

        [HttpPost("registercustomer")]
        public async Task<ActionResult> CreateCustomer([FromBody] RequestCreateCustomerModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is in a valid format
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Email must be in a valid format." });
            }

            // Check if the password has at least 8 characters
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters long." });
            }

            // Check if the email already exists
            var existingCustomerByEmail = _unitOfWork.CustomerRepository.Get(c => c.Email == request.Email).FirstOrDefault();
            if (existingCustomerByEmail != null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password, // Consider hashing the password here
                Phone = request.Phone,
                Address = request.Address,
                RegistrationDate = DateTime.Now, // Set to current date
                Status = "Active",
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            _unitOfWork.CustomerRepository.Insert(customer);
            await _unitOfWork.SaveAsync();

            return Ok("Đăng ký thành công");
        }
        private bool IsValidEmail(string email)
        {
            // Validate email format (must end with @gmail.com)
            return !string.IsNullOrWhiteSpace(email) && email.EndsWith("@gmail.com");
        }

        // POST: api/auth/logincustomer
        [HttpPost("logincustomer")]
        public IActionResult LoginCustomer([FromBody] LoginModelCustomer loginModel)
        {
            // Kiểm tra trong bảng Customer
            var customer = _unitOfWork.CustomerRepository.Get(x => x.Email == loginModel.Email && x.Password == loginModel.Password && x.DeleteStatus == false).FirstOrDefault();

            if (customer != null)
            {
                var token = _tokenService.GenerateToken(customer.Email, "Customer");
                return Ok(new { token, customerId = customer.CustomerId, customerName = customer.Name, Role = "Customer" });
            }

            return Unauthorized("Invalid email or password.");
        }
    }
}

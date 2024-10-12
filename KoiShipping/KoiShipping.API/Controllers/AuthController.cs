using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using KoiShipping.API.Models.LoginModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using System.Linq;
using KoiShipping.API.Models.CustomerModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Policy;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
            // Tìm nhân viên dựa trên email
            var staff = _unitOfWork.StaffRepository.Get(x => x.Email == loginModel.Email && x.DeleteStatus == false).FirstOrDefault();

            if (staff == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Khởi tạo PasswordHasher để kiểm tra mật khẩu
            var passwordHasher = new PasswordHasher<Staff>();

            // Xác thực mật khẩu đã nhập
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(staff, staff.Password, loginModel.Password);

            // Kiểm tra kết quả xác thực
            if (passwordVerificationResult == PasswordVerificationResult.Success)
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

            // Hash the password using PasswordHasher
            var passwordHasher = new PasswordHasher<Customer>();
            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                RegistrationDate = DateTime.Now, // Set to current date
                Status = "Active",
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            // Hash the password before saving it to the database
            customer.Password = passwordHasher.HashPassword(customer, request.Password);

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
            // Tìm khách hàng dựa trên email
            var customer = _unitOfWork.CustomerRepository.Get(x => x.Email == loginModel.Email && x.DeleteStatus == false).FirstOrDefault();

            if (customer == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Khởi tạo PasswordHasher để kiểm tra mật khẩu
            var passwordHasher = new PasswordHasher<Customer>();

            // Xác thực mật khẩu đã nhập
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(customer, customer.Password, loginModel.Password);

            // Kiểm tra kết quả xác thực
            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                var token = _tokenService.GenerateToken(customer.Email, "Customer");
                return Ok(new { token, customerId = customer.CustomerId, customerName = customer.Name, Role = "Customer" });
            }

            return Unauthorized("Invalid email or password.");
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse") // Vẫn giữ URL này để xử lý xác thực
            };

            properties.Items["scope"] = "openid profile email"; // Yêu cầu quyền email
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest("Authentication failed.");
            }

            var userEmail = result.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = result.Principal.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;

            if (userEmail == null || userName == null)
            {
                return BadRequest("Email or Name is not valid.");
            }

            var existingCustomer = await _unitOfWork.CustomerRepository.GetByEmailAsync(userEmail);

            if (existingCustomer == null)
            {
                var newCustomer = new Customer
                {
                    Name = userName,
                    Email = userEmail,
                    Password = "string123", // Default password
                    RegistrationDate = DateTime.UtcNow,
                    Status = "Active",
                    DeleteStatus = false
                };

                _unitOfWork.CustomerRepository.Insert(newCustomer);
                await _unitOfWork.SaveAsync();

                existingCustomer = newCustomer;
            }

            var role = "Customer";
            var tokenService = HttpContext.RequestServices.GetRequiredService<TokenService>();
            var token = tokenService.GenerateToken(existingCustomer.Email, role);

            // Ghi token vào cookie (có thể sử dụng Session hoặc local storage)
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Bật Secure nếu bạn đang sử dụng HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7) // Hoặc thời gian khác tùy ý
            });

            // Chuyển hướng về homepage
            return Redirect("https://chatgpt.com"); // Thay đổi URL này nếu cần
        }

        [HttpGet("get-customer-info")]
        public async Task<IActionResult> GetCustomerInfo()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest("Authentication failed.");
            }

            var userEmail = result.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return BadRequest("Email is not valid.");
            }

            // Lấy thông tin khách hàng từ cơ sở dữ liệu
            var existingCustomer = await _unitOfWork.CustomerRepository.GetByEmailAsync(userEmail);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            var role = "Customer";
            var tokenService = HttpContext.RequestServices.GetRequiredService<TokenService>();
            var token = tokenService.GenerateToken(existingCustomer.Email, role);

            // Trả về phản hồi với token và thông tin khách hàng
            return Ok(new
            {
                token,
                customerId = existingCustomer.CustomerId,
                customerName = existingCustomer.Name,
                Role = role
            });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("https://chatgpt.com/"); // Chuyển hướng đến URL bên ngoài
        }

    }
}

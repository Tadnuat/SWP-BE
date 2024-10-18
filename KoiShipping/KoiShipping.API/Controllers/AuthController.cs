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
using Google.Apis.Gmail.v1;

namespace KoiShipping.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AuthController(IUnitOfWork unitOfWork, TokenService tokenService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailService = emailService;
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
                Status = "Active", // Giữ trạng thái "Pending" cho đến khi OTP được xác nhận
                DeleteStatus = true // Đặt DeleteStatus là true cho đến khi OTP được xác nhận
            };

            // Hash the password before saving it to the database
            customer.Password = passwordHasher.HashPassword(customer, request.Password);

            // Generate OTP
            var otp = GenerateOtp();
            customer.Otp = otp; // Lưu OTP vào cơ sở dữ liệu (giới hạn 6 ký tự)
            customer.OtpExpiration = DateTime.Now.AddMinutes(10); // OTP sẽ hết hạn sau 10 phút

            _unitOfWork.CustomerRepository.Insert(customer);
            await _unitOfWork.SaveAsync();

            // Gửi OTP qua email
            var emailSent = await _emailService.SendEmailAsync(request.Email, "Your OTP Code", $"Your OTP code is {otp}");
            if (!emailSent)
            {
                return StatusCode(500, "Failed to send OTP email.");
            }

            return Ok(new { message = "Đăng ký thành công. Vui lòng kiểm tra email để nhận OTP." });
        }
        [HttpPost("verifyotp")]
        public async Task<ActionResult> VerifyOtp(string email, string otp)
        {
            var customer = _unitOfWork.CustomerRepository.Get(c => c.Email == email).FirstOrDefault();
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            // Kiểm tra thời gian hết hạn OTP
            if (customer.OtpExpiration < DateTime.Now)
            {
                return BadRequest(new { message = "OTP has expired." });
            }

            // Kiểm tra OTP có khớp hay không
            if (customer.Otp != otp)
            {
                return BadRequest(new { message = "Invalid OTP." });
            }

            // Xác nhận OTP thành công, cập nhật DeleteStatus
            customer.DeleteStatus = false; // Đặt DeleteStatus thành false sau khi xác nhận thành công
            customer.Otp = null; // Xóa OTP sau khi xác nhận thành công
            customer.OtpExpiration = null;

            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return Ok(new { message = "OTP verified successfully. Your account is now active." });
        }
        private bool IsValidEmail(string email)
        {
            // Validate email format (must end with @gmail.com)
            return !string.IsNullOrWhiteSpace(email) && email.EndsWith("@gmail.com");
        }
        private string GenerateOtp()
        {
            // Implement OTP generation logic (e.g., a random 6-digit number)
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Simple OTP generation
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
                RedirectUri = Url.Action("GoogleResponse")
            };

            // Chỉ định các quyền cần thiết
            properties.Items["scope"] = "openid profile email"; // Yêu cầu quyền email
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest("Authentication failed."); // Nếu không xác thực thành công
            }

            var userEmail = result.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = result.Principal.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;

            if (userEmail == null || userName == null)
            {
                return BadRequest("Email or Name is not valid."); // Kiểm tra email và tên
            }

            // Kiểm tra xem người dùng đã tồn tại trong cơ sở dữ liệu chưa
            var existingCustomer = await _unitOfWork.CustomerRepository.GetByEmailAsync(userEmail); // Phương thức tìm kiếm khách hàng theo email

            if (existingCustomer == null)
            {
                // Tạo tài khoản mới
                var newCustomer = new Customer
                {
                    Name = userName,
                    Email = userEmail,
                    Password = "string123", // Mật khẩu mặc định
                    RegistrationDate = DateTime.UtcNow, // Ngày đăng ký
                    Status = "Active", // Hoặc giá trị khác tùy theo yêu cầu
                    DeleteStatus = false // Đặt DeleteStatus là false
                };

                _unitOfWork.CustomerRepository.Insert(newCustomer);
                await _unitOfWork.SaveAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                // Lấy thông tin của khách hàng mới tạo
                existingCustomer = newCustomer;
            }

            // Tạo token JWT cho người dùng
            var role = "Customer"; // Vai trò của khách hàng
            var tokenService = HttpContext.RequestServices.GetRequiredService<TokenService>();
            var token = tokenService.GenerateToken(existingCustomer.Email, role);

            // Lưu token vào cookie nếu cần
            HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            // Chuyển hướng người dùng tới trang chủ (homepage)
            return Redirect($"http://localhost:3000/home?token={token}&userId={existingCustomer.CustomerId}&role={role}");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("http://localhost:3000/home"); // Chuyển hướng đến URL bên ngoài
        }

    }
}

using Google.Apis.Gmail.v1;
using KoiShipping.API.Models.PasswordModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork; // Sử dụng UnitOfWork hoặc repository
        private readonly IEmailService _emailService; // Service for sending emails

        public PasswordController(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        [HttpPost("staff/change-password/{StaffId}")]
        public async Task<IActionResult> ChangeStaffPassword(int StaffId, [FromBody] ChangePasswordModel model)
        {
            var staff = _unitOfWork.StaffRepository.GetByID(StaffId); // Lấy thông tin staff từ repo
            if (staff == null)
            {
                return NotFound("Staff not found.");
            }

            // Tạo PasswordHasher cho Staff
            var passwordHasher = new PasswordHasher<Staff>();

            // Kiểm tra mật khẩu cũ bằng cách so sánh với mật khẩu hash
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(staff, staff.Password, model.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Old password is incorrect.");
            }

            // Cập nhật mật khẩu mới sau khi hash
            staff.Password = passwordHasher.HashPassword(staff, model.NewPassword);
            _unitOfWork.StaffRepository.Update(staff); // Cập nhật staff qua repository
            await _unitOfWork.SaveAsync();

            return Ok("Password changed successfully.");
        }
        [HttpPost("customer/change-password/{customerId}")]
        public async Task<IActionResult> ChangeCustomerPassword(int customerId, [FromBody] ChangePasswordModel model)
        {
            var customer = _unitOfWork.CustomerRepository.GetByID(customerId); // Lấy thông tin customer từ repo
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Tạo PasswordHasher cho Customer
            var passwordHasher = new PasswordHasher<Customer>();

            // Kiểm tra mật khẩu cũ bằng cách so sánh với mật khẩu hash
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(customer, customer.Password, model.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Old password is incorrect.");
            }

            // Cập nhật mật khẩu mới sau khi hash
            customer.Password = passwordHasher.HashPassword(customer, model.NewPassword);
            _unitOfWork.CustomerRepository.Update(customer); // Cập nhật customer qua repository
            await _unitOfWork.SaveAsync();

            return Ok("Password changed successfully.");
        }
        // Method to request a password reset
        [HttpPost("customer/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByEmailAsync(model.Email); // Assuming you have a method to get customer by email
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Generate OTP and store it (you may want to save it in a database with an expiry time)
            var otp = GenerateOtp(); // You should implement this method
            customer.Otp = otp; // Assuming your Customer entity has an Otp property
            await _unitOfWork.SaveAsync();

            // Send OTP via email
            var emailSent = await _emailService.SendEmailAsync(model.Email, "Password Reset OTP", $"Your OTP is: {otp}");
            if (!emailSent)
            {
                return StatusCode(500, "Failed to send email.");
            }

            // Return CustomerId along with success message
            return Ok(new { Message = "OTP sent to your email.", CustomerId = customer.CustomerId });
        }

        // Method to reset password using OTP
        [HttpPost("customer/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model, int customerId)
        {
            var customer = _unitOfWork.CustomerRepository.GetByID(customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Verify OTP
            if (customer.Otp != model.OTP)
            {
                return BadRequest("Invalid OTP.");
            }

            // Update password
            var passwordHasher = new PasswordHasher<Customer>();
            customer.Password = passwordHasher.HashPassword(customer, model.NewPassword);
            customer.Otp = null; // Clear OTP after use
            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return Ok("Password reset successfully.");
        }

        private string GenerateOtp()
        {
            // Implement OTP generation logic (e.g., a random 6-digit number)
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Simple OTP generation
        }
    }

}

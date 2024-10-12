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

        public PasswordController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

    }
}

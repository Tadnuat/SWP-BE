using KoiShipping.API.Models.PasswordModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
            var staff = _unitOfWork.StaffRepository.GetByID(StaffId); // Sử dụng phương thức GetByID từ GenericRepository
            if (staff == null)
            {
                return NotFound("Staff not found.");
            }

            // Kiểm tra mật khẩu cũ
            if (staff.Password != model.OldPassword)
            {
                return BadRequest("Old password is incorrect.");
            }

            // Cập nhật mật khẩu mới
            staff.Password = model.NewPassword;
            _unitOfWork.StaffRepository.Update(staff); // Cập nhật staff qua repository
            await _unitOfWork.SaveAsync();

            return Ok("Password changed successfully.");
        }

        [HttpPost("customer/change-password/{customerId}")]
        public async Task<IActionResult> ChangeCustomerPassword(int customerId, [FromBody] ChangePasswordModel model)
        {
            var customer = _unitOfWork.CustomerRepository.GetByID(customerId); // Sử dụng phương thức GetByID từ GenericRepository
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Kiểm tra mật khẩu cũ
            if (customer.Password != model.OldPassword)
            {
                return BadRequest("Old password is incorrect.");
            }

            // Cập nhật mật khẩu mới
            customer.Password = model.NewPassword;
            _unitOfWork.CustomerRepository.Update(customer); // Cập nhật customer qua repository
            await _unitOfWork.SaveAsync();

            return Ok("Password changed successfully.");
        }
    }
}

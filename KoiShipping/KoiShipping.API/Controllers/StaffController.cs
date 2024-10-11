using KoiShipping.API.Models.StaffModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager")]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/staff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseStaffModel>>> GetStaffs()
        {
            var staffs = await Task.Run(() => _unitOfWork.StaffRepository.Get().ToList());

            var response = new List<ResponseStaffModel>();

            foreach (var staff in staffs)
            {
                response.Add(new ResponseStaffModel
                {
                    StaffId = staff.StaffId,
                    StaffName = staff.StaffName,
                    Email = staff.Email,
                    Phone = staff.Phone,
                    Role = staff.Role,
                    Status = staff.Status,
                    DeleteStatus = staff.DeleteStatus
                });
            }

            return Ok(response);
        }
        // GET: api/staff/active
        [HttpGet("status/active")]
        public async Task<ActionResult<IEnumerable<ResponseStaffModel>>> GetActiveStaffs()
        {
            // Lấy danh sách nhân viên có Status là "active" và DeleteStatus là false
            var activeStaffs = await Task.Run(() => _unitOfWork.StaffRepository
                .Get(s => s.Status.ToLower() == "active" && !s.DeleteStatus && s.Role == "Delivering Staff")
                .ToList());

            var response = new List<ResponseStaffModel>();

            foreach (var staff in activeStaffs)
            {
                response.Add(new ResponseStaffModel
                {
                    StaffId = staff.StaffId,
                    StaffName = staff.StaffName,
                    Email = staff.Email,
                    Phone = staff.Phone,
                    Role = staff.Role,
                    Status = staff.Status,
                    DeleteStatus = staff.DeleteStatus
                });
            }

            return Ok(response);
        }


        // GET: api/staff/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseStaffModel>> GetStaff(int id)
        {
            var staff = _unitOfWork.StaffRepository.GetByID(id);

            if (staff == null || staff.DeleteStatus)
            {
                return NotFound();
            }

            var response = new ResponseStaffModel
            {
                StaffId = staff.StaffId,
                StaffName = staff.StaffName,
                Email = staff.Email,
                Phone = staff.Phone,
                Role = staff.Role,
                Status = staff.Status,
                DeleteStatus = staff.DeleteStatus
            };

            return Ok(response);
        }

        // POST: api/staff
        [HttpPost]
        public async Task<ActionResult> CreateStaff([FromBody] RequestCreateStaffModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate email format
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Email must be in a valid format." });
            }

            // Check if the password has at least 8 characters
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters long." });
            }

            // Ensure the role is valid
            if (string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { message = "Role cannot be empty." });
            }

            // Check if the email already exists
            var existingStaffByEmail = _unitOfWork.StaffRepository.Get(s => s.Email == request.Email).FirstOrDefault();
            if (existingStaffByEmail != null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            // Hash the password using PasswordHasher
            var passwordHasher = new PasswordHasher<Staff>();
            var staff = new Staff
            {
                StaffName = request.StaffName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                Status = request.Status,
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            // Hash the password before saving it to the database
            staff.Password = passwordHasher.HashPassword(staff, request.Password);

            _unitOfWork.StaffRepository.Insert(staff);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetStaff), new { id = staff.StaffId }, staff);
        }

        // PUT: api/staff/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] RequestUpdateStaffModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem staff có tồn tại không
            var existingStaff = _unitOfWork.StaffRepository.GetByID(id);
            if (existingStaff == null)
            {
                return NotFound();
            }

            // Kiểm tra email có đúng định dạng không
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Email must be in a valid format." });
            }

            // Kiểm tra email có tồn tại không (nếu email đã thay đổi)
            if (existingStaff.Email != request.Email)
            {
                var existingStaffByEmail = _unitOfWork.StaffRepository.Get(s => s.Email == request.Email).FirstOrDefault();
                if (existingStaffByEmail != null)
                {
                    return Conflict(new { message = "Email already exists." });
                }
            }

            // Cập nhật các thông tin
            existingStaff.StaffName = request.StaffName;
            existingStaff.Email = request.Email;
            //existingStaff.Password = request.Password;
            existingStaff.Phone = request.Phone;
            existingStaff.Role = request.Role;
            existingStaff.Status = request.Status;
            existingStaff.DeleteStatus = request.DeleteStatus;

            // Lưu thay đổi
            _unitOfWork.StaffRepository.Update(existingStaff);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }


        // DELETE: api/staff/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = _unitOfWork.StaffRepository.GetByID(id);

            if (staff == null)
            {
                return NotFound();
            }

            _unitOfWork.StaffRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/staff/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteStaff(int id)
        {
            var staff = _unitOfWork.StaffRepository.GetByID(id);

            if (staff == null)
            {
                return NotFound();
            }

            if (staff.DeleteStatus)
            {
                return BadRequest("Staff is already marked as deleted.");
            }

            _unitOfWork.StaffRepository.SoftDelete(staff);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
        private bool IsValidEmail(string email)
        {
            // Validate email format (must end with @gmail.com)
            return !string.IsNullOrWhiteSpace(email) && email.EndsWith("@gmail.com");
        }
    }
}

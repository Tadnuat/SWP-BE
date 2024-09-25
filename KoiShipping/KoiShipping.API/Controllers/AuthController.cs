using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using KoiShipping.API.Models.LoginModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using System.Linq;

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
                return Ok(new { token, id = staff.StaffId, Username = staff.StaffName, Role = staff.Role });
            }

            return Unauthorized("Invalid email or password.");
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
                return Ok(new { token, customerName = customer.Name, customerId = customer.CustomerId });
            }

            return Unauthorized("Invalid email or password.");
        }
    }
}

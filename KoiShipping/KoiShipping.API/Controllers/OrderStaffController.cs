using KoiShipping.API.Models.OrderStaffModel;
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
    public class OrderStaffController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderStaffController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/orderstaff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseOrderStaffModel>>> GetOrderStaffs()
        {
            var orderStaffs = await Task.Run(() => _unitOfWork.OrderStaffRepository.Get().ToList());

            var response = orderStaffs.Select(os => new ResponseOrderStaffModel
            {
                OrderStaffsId = os.OrderStaffsId,
                OrderId = os.OrderId,
                StaffId = os.StaffId,
            }).ToList();

            return Ok(response);
        }

        // GET: api/orderstaff/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOrderStaffModel>> GetOrderStaff(int id)
        {
            var orderStaff = _unitOfWork.OrderStaffRepository.GetByID(id);

            if (orderStaff == null)
            {
                return NotFound();
            }

            var response = new ResponseOrderStaffModel
            {
                OrderStaffsId = orderStaff.OrderStaffsId,
                OrderId = orderStaff.OrderId,
                StaffId = orderStaff.StaffId,

            };

            return Ok(response);
        }

        // POST: api/orderstaff
        [HttpPost]
        public async Task<ActionResult> CreateOrderStaff([FromBody] RequestCreateOrderStaffModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderStaff = new OrderStaff
            {
                OrderId = request.OrderId,
                StaffId = request.StaffId,
            };

            _unitOfWork.OrderStaffRepository.Insert(orderStaff);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetOrderStaff), new { id = orderStaff.OrderStaffsId }, orderStaff);
        }

        // PUT: api/orderstaff/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStaff(int id, [FromBody] RequestUpdateOrderStaffModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderStaff = _unitOfWork.OrderStaffRepository.GetByID(id);

            if (orderStaff == null)
            {
                return NotFound();
            }

            orderStaff.OrderId = request.OrderId;
            orderStaff.StaffId = request.StaffId;

            _unitOfWork.OrderStaffRepository.Update(orderStaff);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/orderstaff/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStaff(int id)
        {
            var orderStaff = _unitOfWork.OrderStaffRepository.GetByID(id);

            if (orderStaff == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderStaffRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/orderstaff/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteOrderStaff(int id)
        {
            var orderStaff = _unitOfWork.OrderStaffRepository.GetByID(id);

            if (orderStaff == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderStaffRepository.SoftDelete(orderStaff);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

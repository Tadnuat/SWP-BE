using KoiShipping.API.Models.OrderDetailModel;
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
    [Authorize(Roles = "Manager,Staff")]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/orderdetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetails()
        {
            // Retrieve only order details where DeleteStatus is false
            var orderDetails = await Task.Run(() => _unitOfWork.OrderDetailRepository.Get().Where(od => !od.DeleteStatus).ToList());

            var response = orderDetails.Select(od => new ResponseOrderDetailModel
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                CustomerId = od.CustomerId,
                ServiceId = od.ServiceId,
                Weight = od.Weight,
                Quantity = od.Quantity,
                Price = od.Price,
                KoiStatus = od.KoiStatus,
                AttachedItem = od.AttachedItem,
                Status = od.Status,
                DeleteStatus = od.DeleteStatus,
                ReceiverName = od.ReceiverName,
                ReceiverPhone = od.ReceiverPhone,
                Rating = od.Rating,
                Feedback = od.Feedback
            }).ToList();

            return Ok(response);
        }

        // GET: api/orderdetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOrderDetailModel>> GetOrderDetail(int id)
        {
            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);

            // Check if the order detail exists and is not marked as deleted
            if (orderDetail == null || orderDetail.DeleteStatus)
            {
                return NotFound();
            }

            var response = new ResponseOrderDetailModel
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                CustomerId = orderDetail.CustomerId,
                ServiceId = orderDetail.ServiceId,
                Weight = orderDetail.Weight,
                Quantity = orderDetail.Quantity,
                Price = orderDetail.Price,
                KoiStatus = orderDetail.KoiStatus,
                AttachedItem = orderDetail.AttachedItem,
                Status = orderDetail.Status,
                DeleteStatus = orderDetail.DeleteStatus,
                ReceiverName = orderDetail.ReceiverName,
                ReceiverPhone = orderDetail.ReceiverPhone,
                Rating = orderDetail.Rating,
                Feedback = orderDetail.Feedback
            };

            return Ok(response);
        }

        // POST: api/orderdetail
        [HttpPost]
        public async Task<ActionResult> CreateOrderDetail([FromBody] RequestCreateOrderDetailModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = new OrderDetail
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                ServiceId = request.ServiceId,
                Weight = request.Weight,
                Quantity = request.Quantity,
                Price = request.Price,
                KoiStatus = request.KoiStatus,
                AttachedItem = request.AttachedItem,
                Status = request.Status,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                Rating = request.Rating, // Include Rating
                Feedback = request.Feedback, // Include Feedback
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            _unitOfWork.OrderDetailRepository.Insert(orderDetail);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.OrderDetailId }, orderDetail);
        }


        // PUT: api/orderdetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] RequestUpdateOrderDetailModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            // Update fields only if they are provided
            if (request.CustomerId.HasValue) orderDetail.CustomerId = request.CustomerId.Value;
            if (request.ServiceId.HasValue) orderDetail.ServiceId = request.ServiceId.Value;
            if (request.Weight.HasValue) orderDetail.Weight = request.Weight.Value;
            if (request.Quantity.HasValue) orderDetail.Quantity = request.Quantity.Value;
            if (request.Price.HasValue) orderDetail.Price = request.Price.Value;
            if (!string.IsNullOrWhiteSpace(request.KoiStatus)) orderDetail.KoiStatus = request.KoiStatus;
            if (!string.IsNullOrWhiteSpace(request.AttachedItem)) orderDetail.AttachedItem = request.AttachedItem;
            if (!string.IsNullOrWhiteSpace(request.Status)) orderDetail.Status = request.Status;
            if (!string.IsNullOrWhiteSpace(request.ReceiverName)) orderDetail.ReceiverName = request.ReceiverName;
            if (!string.IsNullOrWhiteSpace(request.ReceiverPhone)) orderDetail.ReceiverPhone = request.ReceiverPhone;
            if (request.Rating.HasValue) orderDetail.Rating = request.Rating.Value; // Update Rating if provided
            if (!string.IsNullOrWhiteSpace(request.Feedback)) orderDetail.Feedback = request.Feedback; // Update Feedback if provided

            // Set DeleteStatus to false regardless of the request body
            orderDetail.DeleteStatus = false;

            _unitOfWork.OrderDetailRepository.Update(orderDetail);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }


        // DELETE: api/orderdetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderDetailRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/orderdetail/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteOrderDetail(int id)
        {
            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            if (orderDetail.DeleteStatus)
            {
                return BadRequest("OrderDetail is already marked as deleted.");
            }

            _unitOfWork.OrderDetailRepository.SoftDelete(orderDetail);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

using KoiShipping.API.Models.OrderModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager,Staff")]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseOrderModel>>> GetOrders()
        {
            // Retrieve only orders where DeleteStatus is false
            var orders = await Task.Run(() => _unitOfWork.OrderRepository.Get().Where(o => !o.DeleteStatus).ToList());

            var response = new List<ResponseOrderModel>();

            foreach (var order in orders)
            {
                response.Add(new ResponseOrderModel
                {
                    OrderId = order.OrderId,
                    StartLocation = order.StartLocation,
                    Destination = order.Destination,
                    TransportMethod = order.TransportMethod,
                    DepartureDate = order.DepartureDate,
                    ArrivalDate = order.ArrivalDate,
                    Status = order.Status,
                    TotalWeight = order.TotalWeight,
                    TotalKoiFish = order.TotalKoiFish,
                    StaffId = order.StaffId,
                    DeleteStatus = order.DeleteStatus
                });
            }

            return Ok(response);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOrderModel>> GetOrder(int id)
        {
            var order = _unitOfWork.OrderRepository.GetByID(id);

            // Check if the order exists and is not marked as deleted
            if (order == null || order.DeleteStatus)
            {
                return NotFound();
            }

            var response = new ResponseOrderModel
            {
                OrderId = order.OrderId,
                StartLocation = order.StartLocation,
                Destination = order.Destination,
                TransportMethod = order.TransportMethod,
                DepartureDate = order.DepartureDate,
                ArrivalDate = order.ArrivalDate,
                Status = order.Status,
                TotalWeight = order.TotalWeight,
                TotalKoiFish = order.TotalKoiFish,
                StaffId = order.StaffId,
                DeleteStatus = order.DeleteStatus
            };

            return Ok(response);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] RequestCreateOrderModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                OrderId = request.OrderId,
                StartLocation = request.StartLocation,
                Destination = request.Destination,
                TransportMethod = request.TransportMethod,
                DepartureDate = request.DepartureDate,
                Status = request.Status,
                TotalWeight = request.TotalWeight,
                TotalKoiFish = request.TotalKoiFish,
                StaffId = request.StaffId,
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            _unitOfWork.OrderRepository.Insert(order);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // PUT: api/order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] RequestUpdateOrderModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _unitOfWork.OrderRepository.GetByID(id);

            if (order == null)
            {
                return NotFound();
            }

            if (request.StartLocation != null) order.StartLocation = request.StartLocation;
            if (request.Destination != null) order.Destination = request.Destination;
            if (request.TransportMethod != null) order.TransportMethod = request.TransportMethod;
            if (request.DepartureDate.HasValue) order.DepartureDate = request.DepartureDate.Value;
            if (request.Status != null) order.Status = request.Status;
            if (request.TotalWeight.HasValue) order.TotalWeight = request.TotalWeight.Value;
            if (request.TotalKoiFish.HasValue) order.TotalKoiFish = request.TotalKoiFish.Value;

            // Set DeleteStatus to false regardless of the request body
            order.DeleteStatus = false;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = _unitOfWork.OrderRepository.GetByID(id);

            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/order/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteOrder(int id)
        {
            var order = _unitOfWork.OrderRepository.GetByID(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.DeleteStatus)
            {
                return BadRequest("Order is already marked as deleted.");
            }

            _unitOfWork.OrderRepository.SoftDelete(order);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

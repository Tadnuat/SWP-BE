using KoiShipping.API.Models.OrderModel;
using KoiShipping.API.Models.OrderDetailModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            var orders = await _unitOfWork.OrderRepository.GetQueryable()
                .Where(o => !o.DeleteStatus)
                .Include(o => o.OrderStaffs) // Bao gồm bảng OrderStaff
                    .ThenInclude(os => os.Staff) // Bao gồm bảng Staff
                .ToListAsync(); // Sử dụng ToListAsync thay vì ToList

            var response = new List<ResponseOrderModel>();

            foreach (var order in orders)
            {
                var staffDeliveries = order.OrderStaffs.Select(os => new StaffInfo
                {
                    StaffId = os.Staff.StaffId,  // Lấy StaffId
                    StaffName = os.Staff.StaffName // Lấy StaffName
                }).ToList();

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
                    DeleteStatus = order.DeleteStatus,
                    StaffDeliveries = staffDeliveries // Gán danh sách StaffInfo
                });
            }

            return Ok(response);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOrderByIdModel>> GetOrder(int id)
        {
            // Lấy thông tin order cùng với OrderStaff và Staff
            var order = await _unitOfWork.OrderRepository.GetQueryable()
                .Where(o => o.OrderId == id && !o.DeleteStatus)
                .Include(o => o.OrderStaffs)
                    .ThenInclude(os => os.Staff)
                .FirstOrDefaultAsync();

            // Kiểm tra xem đơn hàng có tồn tại hay không
            if (order == null)
            {
                return NotFound();
            }

            // Lấy danh sách OrderDetail liên quan và kết hợp với Customer và Service
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetQueryable()
                .Where(od => od.OrderId == id && !od.DeleteStatus)
                .Include(od => od.Customer) // Giả định rằng có liên kết tới Customer
                .ToListAsync();

            // Lấy thông tin StaffDeliveries từ OrderStaffs
            var staffDeliveries = order.OrderStaffs.Select(os => new StaffInfo
            {
                StaffId = os.Staff.StaffId,
                StaffName = os.Staff.StaffName
            }).ToList();

            // Tạo đối tượng ResponseOrderModel và gán các giá trị
            var response = new ResponseOrderByIdModel
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
                DeleteStatus = order.DeleteStatus,
                StaffDeliveries = staffDeliveries,
                OrderDetails = orderDetails.Select(od => new ResponseOrderDetailModel
                {
                    OrderDetailId = od.OrderDetailId,
                    CustomerId = od.CustomerId,
                    CustomerName = od.Customer?.Name, // Lấy CustomerName từ Customer
                    ServiceId = od.ServiceId,
                    ServiceName = od.ServiceName, // Lấy ServiceName từ Service
                    Weight = od.Weight,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    KoiStatus = od.KoiStatus,
                    AttachedItem = od.AttachedItem,
                    Status = od.Status,
                    ReceiverName = od.ReceiverName,
                    ReceiverPhone = od.ReceiverPhone,
                    Rating = od.Rating,
                    Feedback = od.Feedback,
                    CreatedDate = od.CreatedDate
                }).ToList()
            };

            return Ok(response);
        }

        // POST: api/order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] RequestCreateOrderModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Tạo đối tượng Order
                var order = new Order
                {
                    StartLocation = request.StartLocation,
                    Destination = request.Destination,
                    TransportMethod = request.TransportMethod,
                    DepartureDate = request.DepartureDate,
                    ArrivalDate   = request.ArrivalDate,
                    Status = "Pending",
                    TotalWeight = request.TotalWeight,
                    TotalKoiFish = request.TotalKoiFish,
                    DeleteStatus = false // Đặt DeleteStatus mặc định là false
                };

                _unitOfWork.OrderRepository.Insert(order);
                await _unitOfWork.SaveAsync(); // Lưu để lấy OrderId

                // Kiểm tra và tạo OrderStaff cho từng StaffId
                if (request.StaffIds != null && request.StaffIds.Any())
                {
                    foreach (var staffId in request.StaffIds)
                    {
                        // Kiểm tra xem cặp OrderId và StaffId đã tồn tại trong OrderStaff chưa để tránh trùng lặp
                        var exists = await _unitOfWork.OrderStaffRepository
                            .AnyAsync(os => os.OrderId == order.OrderId && os.StaffId == staffId);

                        if (!exists)
                        {
                            var orderStaff = new OrderStaff
                            {
                                OrderId = order.OrderId,
                                StaffId = staffId
                            };

                            _unitOfWork.OrderStaffRepository.Insert(orderStaff);
                        }
                    }

                    await _unitOfWork.SaveAsync(); // Lưu lại các thay đổi
                }

                // Trả về thông báo thành công
                return Ok("Tạo chuyến thành công");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
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
            if (request.ArrivalDate.HasValue) order.ArrivalDate = request.ArrivalDate.Value;
            if (request.Status != null) order.Status = request.Status;
            if (request.TotalWeight.HasValue) order.TotalWeight = request.TotalWeight.Value;
            if (request.TotalKoiFish.HasValue) order.TotalKoiFish = request.TotalKoiFish.Value;

            order.DeleteStatus = request.DeleteStatus;

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

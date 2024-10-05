using KoiShipping.API.Models.OrderDetailModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager,Staff,Customer")]
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
            // Truy vấn dữ liệu OrderDetail và bao gồm cả bảng trung gian AserviceOrderD và AdvancedService
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetQueryable()
                .Where(od => !od.DeleteStatus)
                .Include(od => od.AserviceOrderDs) // Bao gồm bảng AserviceOrderD
                    .ThenInclude(asod => asod.AdvancedService) // Bao gồm bảng AdvancedService
                .ToListAsync();

            // Lấy danh sách Customer
            var customerIds = orderDetails.Select(od => od.CustomerId).Distinct().ToList();
            var customers = _unitOfWork.CustomerRepository.Get(c => customerIds.Contains(c.CustomerId)).ToList();
            var response = new List<ResponseOrderDetailModel>();

            foreach (var od in orderDetails)
            {
                // Lấy danh sách tên của AdvancedService cho mỗi OrderDetail
                var advancedServiceNames = od.AserviceOrderDs
                    .Select(asod => asod.AdvancedService.AServiceName) // Lấy tên dịch vụ nâng cao
                    .ToList();

                response.Add(new ResponseOrderDetailModel
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    CustomerId = od.CustomerId,
                    CustomerName = customers.FirstOrDefault(c => c.CustomerId == od.CustomerId)?.Name, // Lấy CustomerName
                    StartLocation = od.StartLocation,
                    Destination = od.Destination,
                    ServiceId = od.ServiceId,
                    ServiceName = od.ServiceName,
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
                    Feedback = od.Feedback,
                    CreatedDate = od.CreatedDate,

                    // Gán danh sách tên AdvancedService
                    AdvancedServiceNames = advancedServiceNames
                });
            }

            return Ok(response);
        }

        // GET: api/orderdetail/status/pending
        [HttpGet("status/pending")]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetailByStatusPending()
        {
            var pendingOrderDetails = await _unitOfWork.OrderDetailRepository
                .GetQueryable() // Use GetQueryable to get IQueryable<OrderDetail>
                .Include(od => od.AserviceOrderDs) // Include the related AserviceOrderDs
                    .ThenInclude(asod => asod.AdvancedService) // Include the related AdvancedService
                .Where(od => od.Status.ToLower() == "pending" && !od.DeleteStatus)
                .ToListAsync();

            // Lấy danh sách Customer
            var customerIds = pendingOrderDetails.Select(od => od.CustomerId).Distinct().ToList();
            var customers = await _unitOfWork.CustomerRepository.GetQueryable()
                .Where(c => customerIds.Contains(c.CustomerId))
                .ToListAsync();

            var response = pendingOrderDetails.Select(od => new ResponseOrderDetailModel
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                CustomerId = od.CustomerId,
                CustomerName = customers.FirstOrDefault(c => c.CustomerId == od.CustomerId)?.Name,
                StartLocation = od.StartLocation,
                Destination = od.Destination,
                ServiceId = od.ServiceId,
                ServiceName = od.ServiceName,
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
                Feedback = od.Feedback,
                CreatedDate = od.CreatedDate,
                AdvancedServiceNames = od.AserviceOrderDs.Select(asod => asod.AdvancedService.AServiceName).ToList()
            }).ToList();

            return Ok(response);
        }
        // GET: api/orderdetail/status/pending
        [HttpGet("status/waiting")]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetailByStatusWaiting()
        {
            var pendingOrderDetails = await _unitOfWork.OrderDetailRepository
                .GetQueryable() // Use GetQueryable to get IQueryable<OrderDetail>
                .Include(od => od.AserviceOrderDs) // Include the related AserviceOrderDs
                    .ThenInclude(asod => asod.AdvancedService) // Include the related AdvancedService
                .Where(od => od.Status.ToLower() == "waiting" && !od.DeleteStatus)
                .ToListAsync();

            // Lấy danh sách Customer
            var customerIds = pendingOrderDetails.Select(od => od.CustomerId).Distinct().ToList();
            var customers = await _unitOfWork.CustomerRepository.GetQueryable()
                .Where(c => customerIds.Contains(c.CustomerId))
                .ToListAsync();

            var response = pendingOrderDetails.Select(od => new ResponseOrderDetailModel
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                CustomerId = od.CustomerId,
                CustomerName = customers.FirstOrDefault(c => c.CustomerId == od.CustomerId)?.Name,
                StartLocation = od.StartLocation,
                Destination = od.Destination,
                ServiceId = od.ServiceId,
                ServiceName = od.ServiceName,
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
                Feedback = od.Feedback,
                CreatedDate = od.CreatedDate,
                AdvancedServiceNames = od.AserviceOrderDs.Select(asod => asod.AdvancedService.AServiceName).ToList()
            }).ToList();

            return Ok(response);
        }
        // GET: api/orderdetail/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetailsByCustomerId(int customerId)
        {
            // Lấy danh sách order details theo CustomerId
            var orderDetails = await _unitOfWork.OrderDetailRepository
                .GetQueryable() // Use GetQueryable to get IQueryable<OrderDetail>
                .Include(od => od.AserviceOrderDs)
                    .ThenInclude(asod => asod.AdvancedService)
                .Where(od => od.CustomerId == customerId && !od.DeleteStatus)
                .ToListAsync();

            // Kiểm tra nếu không có OrderDetail
            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound($"No OrderDetails found for CustomerId: {customerId}");
            }

            // Lấy thông tin khách hàng
            var customer = _unitOfWork.CustomerRepository.GetByID(customerId); // Assuming you have an async method

            var response = orderDetails.Select(od => new ResponseOrderDetailModel
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                CustomerId = od.CustomerId,
                CustomerName = customer?.Name,
                StartLocation = od.StartLocation,
                Destination = od.Destination,
                ServiceId = od.ServiceId,
                ServiceName = od.ServiceName,
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
                Feedback = od.Feedback,
                CreatedDate = od.CreatedDate,
                AdvancedServiceNames = od.AserviceOrderDs.Select(asod => asod.AdvancedService.AServiceName).ToList()
            }).ToList();

            return Ok(response);
        }
        // GET: api/orderdetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOrderDetailModel>> GetOrderDetail(int id)
        {
            var orderDetail = await _unitOfWork.OrderDetailRepository
                .GetQueryable() // Use GetQueryable to get IQueryable<OrderDetail>
                .Include(od => od.AserviceOrderDs)
                    .ThenInclude(asod => asod.AdvancedService)
                .FirstOrDefaultAsync(od => od.OrderDetailId == id && !od.DeleteStatus);

            // Check if the order detail exists and is not marked as deleted
            if (orderDetail == null)
            {
                return NotFound();
            }

            // Lấy thông tin khách hàng
            var customer = _unitOfWork.CustomerRepository.GetByID(orderDetail.CustomerId); // Assuming you have an async method

            var response = new ResponseOrderDetailModel
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                CustomerId = orderDetail.CustomerId,
                CustomerName = customer?.Name,
                StartLocation = orderDetail.StartLocation,
                Destination = orderDetail.Destination,
                ServiceId = orderDetail.ServiceId,
                ServiceName = orderDetail.ServiceName,
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
                Feedback = orderDetail.Feedback,
                CreatedDate = orderDetail.CreatedDate,
                AdvancedServiceNames = orderDetail.AserviceOrderDs.Select(asod => asod.AdvancedService.AServiceName).ToList()
            };

            return Ok(response);
        }

        // POST: api/orderdetail
        [HttpPost]
        public async Task<IActionResult> Create(RequestCreateOrderDetailModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tạo đối tượng OrderDetail
            var orderDetail = new OrderDetail
            {
                OrderId = 0, // Cần điều chỉnh nếu cần thiết
                CustomerId = request.CustomerId,
                StartLocation = request.StartLocation,
                Destination = request.Destination,
                ServiceId = request.ServiceId,
                ServiceName = request.ServiceName,
                Weight = request.Weight,
                Quantity = request.Quantity,
                Price = request.Price,
                KoiStatus = request.KoiStatus,
                AttachedItem = request.AttachedItem,
                Status = "Pending",
                DeleteStatus = false,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                CreatedDate = DateTime.Now,
                Rating = null,
                Feedback = null
            };

            try
            {
                // Sử dụng Unit of Work để thêm OrderDetail
                _unitOfWork.OrderDetailRepository.Insert(orderDetail);
                await _unitOfWork.SaveAsync(); // Lưu thay đổi để có OrderDetailId

                // Tạo AserviceOrderD cho từng AdvancedService đã chọn
                if (request.SelectedAdvancedServiceIds != null && request.SelectedAdvancedServiceIds.Any())
                {
                    foreach (var advancedServiceId in request.SelectedAdvancedServiceIds)
                    {
                        // Kiểm tra xem đã tồn tại chưa để tránh lặp
                        var exists = await _unitOfWork.AserviceOrderDRepository
                            .AnyAsync(a => a.OrderDetailId == orderDetail.OrderDetailId && a.AdvancedServiceId == advancedServiceId);

                        if (!exists)
                        {
                            var aserviceOrderD = new AserviceOrderD
                            {
                                OrderDetailId = orderDetail.OrderDetailId,
                                AdvancedServiceId = advancedServiceId
                            };

                            _unitOfWork.AserviceOrderDRepository.Insert(aserviceOrderD);
                        }
                    }

                    await _unitOfWork.SaveAsync(); // Lưu thay đổi cho bảng AserviceOrderD
                }

                // Trả về thông điệp thành công
                return Ok("Tạo đơn thành công"); // Thay đổi phản hồi ở đây
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
            if (request.OrderId.HasValue) orderDetail.OrderId = request.OrderId.Value;
            if (request.CustomerId.HasValue) orderDetail.CustomerId = request.CustomerId.Value;
            if (!string.IsNullOrWhiteSpace(request.StartLocation)) orderDetail.StartLocation = request.StartLocation;
            if (!string.IsNullOrWhiteSpace(request.Destination)) orderDetail.Destination = request.Destination;
            if (request.ServiceId.HasValue) orderDetail.ServiceId = request.ServiceId.Value;
            if (!string.IsNullOrWhiteSpace(request.ServiceName)) orderDetail.ServiceName = request.ServiceName;
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
            orderDetail.DeleteStatus = request.DeleteStatus;

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

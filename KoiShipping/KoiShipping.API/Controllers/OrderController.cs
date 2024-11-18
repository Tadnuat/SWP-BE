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
    [Authorize(Roles = "Manager,Sale Staff,Delivering Staff")]
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
                .Where(o => !o.DeleteStatus).OrderByDescending(o => o.OrderId)
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

            // Truy vấn dữ liệu OrderDetail và bao gồm cả bảng trung gian AserviceOrderD và AdvancedService
            var orderDetail = await _unitOfWork.OrderDetailRepository.GetQueryable()
                .Where(od => !od.DeleteStatus)
                .Include(od => od.AserviceOrderDs) // Bao gồm bảng AserviceOrderD
                    .ThenInclude(asod => asod.AdvancedService) // Bao gồm bảng AdvancedService
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
                DeleteStatus = order.DeleteStatus,
                StaffDeliveries = staffDeliveries,
                OrderDetails = orderDetails.Select(od => new ResponseOrderDetailModel
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    CustomerId = od.CustomerId,
                    CustomerName = od.Customer?.Name,
                    StartLocation = od.StartLocation,
                    Destination = od.Destination,
                    ServiceId = od.ServiceId,
                    ServiceName = od.ServiceName,
                    Weight = od.Weight,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    KoiStatus = od.KoiStatus,
                    AttachedItem = od.AttachedItem,
                    Image = od.Image,
                    ConfirmationImage = od.ConfirmationImage,
                    DeliveryPerson = od.DeliveryPerson,
                    Status = od.Status,
                    DeleteStatus = od.DeleteStatus,
                    ReceiverName = od.ReceiverName,
                    ReceiverPhone = od.ReceiverPhone,
                    Rating = od.Rating,
                    Feedback = od.Feedback,
                    CreatedDate = od.CreatedDate,
                    AdvancedServiceNames = od.AserviceOrderDs.Select(asod => asod.AdvancedService.AServiceName).ToList()
                }).ToList()
            };

            return Ok(response);
        }
        [HttpGet("staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<ResponseOrderModel>>> GetOrdersByStaffId(int staffId)
        {
            // Retrieve orders where the staff with the given staffId is associated and DeleteStatus is false
            var orders = await _unitOfWork.OrderRepository.GetQueryable()
                .Where(o => !o.DeleteStatus && o.OrderStaffs.Any(os => os.StaffId == staffId))
                .OrderByDescending(o => o.OrderId)
                .Include(o => o.OrderStaffs) // Include OrderStaff
                    .ThenInclude(os => os.Staff) // Include Staff
                .ToListAsync();

            var response = new List<ResponseOrderModel>();

            foreach (var order in orders)
            {
                var staffDeliveries = order.OrderStaffs.Select(os => new StaffInfo
                {
                    StaffId = os.Staff.StaffId,
                    StaffName = os.Staff.StaffName
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
                    DeleteStatus = order.DeleteStatus,
                    StaffDeliveries = staffDeliveries
                });
            }

            return Ok(response);
        }

        [HttpGet("order/{orderId}/staff")]
        public async Task<ActionResult<IEnumerable<StaffInfo>>> GetStaffByOrderId(int orderId)
        {
            // Retrieve the order and include associated OrderStaffs and Staff details
            var order = await _unitOfWork.OrderRepository.GetQueryable()
                .Where(o => o.OrderId == orderId && !o.DeleteStatus)
                .Include(o => o.OrderStaffs) // Include OrderStaffs
                    .ThenInclude(os => os.Staff) // Include Staff
                .FirstOrDefaultAsync();

            // If the order does not exist, return NotFound
            if (order == null)
            {
                return NotFound("Order not found or has been deleted.");
            }

            // Select the staff information from the associated OrderStaff records
            var staffDeliveries = order.OrderStaffs.Select(os => new StaffInfo
            {
                StaffId = os.Staff.StaffId,
                StaffName = os.Staff.StaffName
            }).ToList();

            return Ok(staffDeliveries);
        }

        // POST: api/order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] RequestCreateOrderModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra ngày đến không nhỏ hơn ngày xuất phát
            if (request.ArrivalDate < request.DepartureDate)
            {
                return BadRequest("Ngày đến phải lớn hơn ngày xuất phát.");
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
                    Status = "Ready",
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
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ResponseOrderModel>>> GetOrders(
        string? startLocation = null,
        string? destination = null,
        string? transportMethod = null
            )
        {
            // Start with a base query for orders with DeleteStatus as false
            var baseQuery = _unitOfWork.OrderRepository.GetQueryable()
                .Where(o => !o.DeleteStatus && o.Status == "Ready" );

            // Apply filtering if startLocation is provided
            if (!string.IsNullOrWhiteSpace(startLocation))
            {
                baseQuery = baseQuery.Where(o => o.StartLocation.Contains(startLocation));
            }

            // Apply filtering if destination is provided
            if (!string.IsNullOrWhiteSpace(destination))
            {
                baseQuery = baseQuery.Where(o => o.Destination.Contains(destination));
            }
            if (!string.IsNullOrWhiteSpace(destination))
            {
                baseQuery = baseQuery.Where(o => o.TransportMethod.Contains(transportMethod));
            }

            // Include related OrderStaffs and Staff entities
            var orders = await baseQuery
                .OrderByDescending(o => o.OrderId)
                .Include(o => o.OrderStaffs) // Include related OrderStaffs
                    .ThenInclude(os => os.Staff) // Include related Staff
                .ToListAsync();

            // Map the results to the response model
            var response = orders.Select(order => new ResponseOrderModel
            {
                OrderId = order.OrderId,
                StartLocation = order.StartLocation,
                Destination = order.Destination,
                TransportMethod = order.TransportMethod,
                DepartureDate = order.DepartureDate,
                ArrivalDate = order.ArrivalDate,
                Status = order.Status,
                DeleteStatus = order.DeleteStatus,
                StaffDeliveries = order.OrderStaffs.Select(os => new StaffInfo
                {
                    StaffId = os.Staff.StaffId,
                    StaffName = os.Staff.StaffName
                }).ToList()
            }).ToList();

            return Ok(response);
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

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.StartLocation)) order.StartLocation = request.StartLocation;
            if (!string.IsNullOrEmpty(request.Destination)) order.Destination = request.Destination;
            if (!string.IsNullOrEmpty(request.TransportMethod)) order.TransportMethod = request.TransportMethod;
            if (request.DepartureDate.HasValue) order.DepartureDate = request.DepartureDate.Value;
            if (request.ArrivalDate.HasValue) order.ArrivalDate = request.ArrivalDate.Value;
            if (!string.IsNullOrEmpty(request.Status)) order.Status = request.Status;
            order.DeleteStatus = request.DeleteStatus;

            // Update the OrderStaff assignments
            if (request.StaffIds != null && request.StaffIds.Any())
            {
                var currentOrderStaffs = _unitOfWork.OrderStaffRepository.Get()
                                            .Where(os => os.OrderId == id)
                                            .ToList();

                var currentStaffIds = currentOrderStaffs.Select(os => os.StaffId).ToList();

                var staffToRemove = currentOrderStaffs.Where(os => !request.StaffIds.Contains(os.StaffId)).ToList();
                foreach (var orderStaff in staffToRemove)
                {
                    _unitOfWork.OrderStaffRepository.Delete(orderStaff);
                }

                var newStaffIds = request.StaffIds.Except(currentStaffIds).ToList();
                foreach (var staffId in newStaffIds)
                {
                    var newOrderStaff = new OrderStaff
                    {
                        OrderId = id,
                        StaffId = staffId
                    };
                    _unitOfWork.OrderStaffRepository.Insert(newOrderStaff);
                }

                await _unitOfWork.SaveAsync(); // Save changes to OrderStaff
            }

            // If the order status is "finish" or "Finish", update staff status to "active"
            if (request.Status?.Equals("Finish", StringComparison.OrdinalIgnoreCase) == true)
            {
                var orderStaffs = _unitOfWork.OrderStaffRepository.Get()
                                       .Where(os => os.OrderId == id)
                                       .ToList();

                foreach (var orderStaff in orderStaffs)
                {
                    var staff = _unitOfWork.StaffRepository.GetByID(orderStaff.StaffId);
                    if (staff != null)
                    {
                        staff.Status = "Active"; // Update staff status to "active"
                        _unitOfWork.StaffRepository.Update(staff);
                    }
                }

                await _unitOfWork.SaveAsync(); // Save changes to Staff status
            }

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

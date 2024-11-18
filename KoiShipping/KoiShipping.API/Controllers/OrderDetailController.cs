using KoiShipping.API.Models.OrderDetailModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager,Sale Staff,Delivering Staff,Customer")]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderDetailController(IUnitOfWork unitOfWork, IHubContext<OrderHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        // GET: api/orderdetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetails()
        {
            // Truy vấn dữ liệu OrderDetail, sắp xếp theo OrderDetailId giảm dần và bao gồm bảng trung gian AserviceOrderD và AdvancedService
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetQueryable()
                .Where(od => !od.DeleteStatus)
                .OrderByDescending(od => od.OrderDetailId) // Sắp xếp theo OrderDetailId từ cao tới thấp
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

                    // Gán danh sách tên AdvancedService
                    AdvancedServiceNames = advancedServiceNames
                });
            }

            return Ok(response);
        }

        // GET: api/orderdetail/status/waiting
        [HttpGet("status/waiting")]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetailByStatusPending(
     string? startLocation = null,
     string? destination = null,
     string? transportMethod = null
 )
        {
            // Log the initial filter parameters
            Console.WriteLine($"Filter Params - startLocation: {startLocation}, destination: {destination}, transportMethod: {transportMethod}");

            // Start with the base query for pending order details with DeleteStatus as false
            var query = _unitOfWork.OrderDetailRepository
                .GetQueryable()
                .Include(od => od.AserviceOrderDs)
                    .ThenInclude(asod => asod.AdvancedService)
                .Include(od => od.Service) // Include related Service entity
                .Where(od => od.Status.ToLower() == "waiting" && !od.DeleteStatus);

            // Apply filtering if startLocation is provided
            if (!string.IsNullOrWhiteSpace(startLocation))
            {
                query = query.Where(od => od.StartLocation.Contains(startLocation));
                Console.WriteLine("Applied startLocation filter.");
            }

            // Apply filtering if destination is provided
            if (!string.IsNullOrWhiteSpace(destination))
            {
                query = query.Where(od => od.Destination.Contains(destination));
                Console.WriteLine("Applied destination filter.");
            }


            // Apply filtering by transport method through Service entity based on ServiceId
            if (!string.IsNullOrWhiteSpace(transportMethod))
            {
                query = query.Where(od => od.Service != null && od.Service.TransportMethod.ToLower() == transportMethod.ToLower());
                Console.WriteLine("Applied transportMethod filter.");
            }

            // Execute the filtered query and log the count
            var waitingOrderDetails = await query.ToListAsync();
            Console.WriteLine($"Filtered OrderDetails count: {waitingOrderDetails.Count}");

            // Retrieve distinct customer IDs
            var customerIds = waitingOrderDetails.Select(od => od.CustomerId).Distinct().ToList();
            var customers = await _unitOfWork.CustomerRepository.GetQueryable()
                .Where(c => customerIds.Contains(c.CustomerId))
                .ToListAsync();

            // Map to the response model
            var response = waitingOrderDetails.Select(od => new ResponseOrderDetailModel
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
            }).ToList();

            // Log final response count
            Console.WriteLine($"Response OrderDetails count: {response.Count}");

            return Ok(response);
        }

        // GET: api/orderdetail/status/pending
        [HttpGet("status/pending")]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetailByStatusWaiting()
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
            }).ToList();

            return Ok(response);
        }
        // GET: api/orderdetail/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<ResponseOrderDetailModel>>> GetOrderDetailsByCustomerId(int customerId)
        {
            // Lấy danh sách order details theo CustomerId và sắp xếp theo OrderDetailId từ lớn tới nhỏ
            var orderDetails = await _unitOfWork.OrderDetailRepository
                .GetQueryable() // Use GetQueryable to get IQueryable<OrderDetail>
                .Include(od => od.AserviceOrderDs)
                    .ThenInclude(asod => asod.AdvancedService)
                .Where(od => od.CustomerId == customerId && !od.DeleteStatus)
                .OrderByDescending(od => od.OrderDetailId) // Sắp xếp theo OrderDetailId từ lớn đến nhỏ
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
            }).ToList();

            return Ok(response);
        }

        // GET: api/orderdetail/deliveryperson/{deliveryPersonName}
        [HttpGet("deliveryperson/{deliveryPersonName}")]
        public async Task<ActionResult<IEnumerable<ResponseDeliverystaffOrderDetailModel>>> GetOrderDetailsByDeliveryPerson(string deliveryPersonName)
        {
            var orderDetails = await _unitOfWork.OrderDetailRepository
                .GetQueryable()
                .Include(od => od.AserviceOrderDs)
                    .ThenInclude(asod => asod.AdvancedService)
                .Include(od => od.Order) // Include Order để lấy thông tin Status
                .Include(od => od.Customer) // Include Customer để lấy CustomerName
                .Where(od => !string.IsNullOrEmpty(od.DeliveryPerson) &&
                             od.DeliveryPerson.ToLower() == deliveryPersonName.ToLower() &&
                             !od.DeleteStatus)
                .OrderByDescending(od => od.OrderDetailId)
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound($"No OrderDetails found for DeliveryPerson: {deliveryPersonName}");
            }

            var response = orderDetails.Select(od => new ResponseDeliverystaffOrderDetailModel
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                CustomerId = od.CustomerId,
                CustomerName = od.Customer?.Name, // Lấy thông tin tên từ Customer
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
                AdvancedServiceNames = od.AserviceOrderDs?.Select(asod => asod.AdvancedService?.AServiceName).ToList() ?? new List<string>(),
                IsDone = od.Order?.Status.ToLower() == "finish" // Xác định trạng thái hoàn thành
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
                Image = orderDetail.Image,
                ConfirmationImage = orderDetail.ConfirmationImage,
                DeliveryPerson = orderDetail.DeliveryPerson,
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
                Image = request.Image,
                Status = "Pending",
                DeleteStatus = false,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                Rating = null,
                Feedback = null
            };

            try
            {
                // Sử dụng Unit of Work để thêm OrderDetail
                _unitOfWork.OrderDetailRepository.Insert(orderDetail);
                await _unitOfWork.SaveAsync(); // Lưu thay đổi để có OrderDetailId

                // Gửi thông báo đến tất cả các client đang kết nối
                var message = $"Đơn hàng mới từ khách hàng với mã đơn hàng: {orderDetail.OrderDetailId}";
                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", message);

                // Tạo Notification mới
                var notification = new Notification
                {
                    Message = message,
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    IsRead = false,
                    Role = "Staffs",
                    CustomerId = 0
                };

                // Thêm Notification vào cơ sở dữ liệu
                _unitOfWork.NotificationRepository.Insert(notification);
                await _unitOfWork.SaveAsync(); // Lưu thay đổi cho bảng Notification

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

                var trackingOrderD = new TrackingOrderD
                {
                    OrderDetailId = orderDetail.OrderDetailId,
                    TrackingId = 1, // Giá trị mặc định là 1
                    Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                };

                _unitOfWork.TrackingOrderDRepository.Insert(trackingOrderD);
                await _unitOfWork.SaveAsync(); // Lưu thay đổi cho bảng TrackingOrderD

                // Trả về thông điệp thành công
                return Ok("Tạo đơn thành công"); // Thay đổi phản hồi ở đây
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // POST: api/orderdetail/copy/{id}
        [HttpPost("copy/{id}")]
        public async Task<IActionResult> CopyOrderDetail(int id)
        {
            try
            {
                // Fetch the existing OrderDetail by ID
                var existingOrderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);
                if (existingOrderDetail == null)
                {
                    return NotFound("OrderDetail not found");
                }

                // Fetch the associated Customer information
                var customer = _unitOfWork.CustomerRepository.GetByID(existingOrderDetail.CustomerId);
                if (customer == null)
                {
                    return NotFound("Associated Customer not found");
                }

                // Update the status of the original OrderDetail to "refund"
                existingOrderDetail.Status = "Refund";
                _unitOfWork.OrderDetailRepository.Update(existingOrderDetail);

                // Create a new OrderDetail based on the existing one
                var newOrderDetail = new OrderDetail
                {
                    CustomerId = existingOrderDetail.CustomerId,
                    StartLocation = existingOrderDetail.Destination,
                    Destination = existingOrderDetail.StartLocation,
                    ServiceId = existingOrderDetail.ServiceId,
                    ServiceName = existingOrderDetail.ServiceName,
                    Weight = existingOrderDetail.Weight,
                    Quantity = existingOrderDetail.Quantity,
                    Price = existingOrderDetail.Price / 2,  // Halve the price for the new OrderDetail
                    KoiStatus = "Đơn Hoàn Trả",
                    AttachedItem = existingOrderDetail.AttachedItem,
                    Image = existingOrderDetail.Image,
                    Status = "Pending",
                    DeleteStatus = false,
                    ReceiverName = customer.Name,
                    ReceiverPhone = customer.Phone,
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    Rating = null,
                    Feedback = null
                };

                // Insert the new OrderDetail
                _unitOfWork.OrderDetailRepository.Insert(newOrderDetail);
                await _unitOfWork.SaveAsync(); // Save to generate OrderDetailId

                // Create a new Notification for Staffs
                var staffMessage = $"Đơn hàng hoàn trả của khách hàng: {newOrderDetail.ReceiverName} với mã đơn hàng: {newOrderDetail.OrderDetailId}";
                var staffNotification = new Notification
                {
                    Message = staffMessage,
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    IsRead = false,
                    Role = "Staffs",
                    CustomerId = 0
                };
                _unitOfWork.NotificationRepository.Insert(staffNotification);

                // Create a new Notification for the customer
                var customerMessage = $"Đơn hàng {newOrderDetail.OrderDetailId} đã được chuyển sang trạng thái hoàn trả. - {newOrderDetail.OrderDetailId}";
                var customerNotification = new Notification
                {
                    Message = customerMessage,
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    IsRead = false,
                    Role = "Customer",
                    CustomerId = newOrderDetail.CustomerId
                };
                _unitOfWork.NotificationRepository.Insert(customerNotification);

                // Send notifications via SignalR to connected clients
                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", staffMessage);
                await _hubContext.Clients.All.SendAsync("NotiCustomer", customerMessage);

                // Duplicate TrackingOrderD for the new OrderDetail
                var trackingOrderD = new TrackingOrderD
                {
                    OrderDetailId = newOrderDetail.OrderDetailId,
                    TrackingId = 1, // Default tracking ID
                    Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                };
                _unitOfWork.TrackingOrderDRepository.Insert(trackingOrderD);

                // Duplicate AserviceOrderD records for the new OrderDetail
                var existingAdvancedServices = _unitOfWork.AserviceOrderDRepository
                    .Get(a => a.OrderDetailId == existingOrderDetail.OrderDetailId);

                foreach (var aserviceOrderD in existingAdvancedServices)
                {
                    var newAserviceOrderD = new AserviceOrderD
                    {
                        OrderDetailId = newOrderDetail.OrderDetailId,
                        AdvancedServiceId = aserviceOrderD.AdvancedServiceId
                    };
                    _unitOfWork.AserviceOrderDRepository.Insert(newAserviceOrderD);
                }

                await _unitOfWork.SaveAsync(); // Save changes to Notification, TrackingOrderD, and AserviceOrderD tables

                // Return a success message
                return Ok("Order detail duplicated successfully, with associated notifications and services.");
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost("update")]
        public async Task UpdateOrderDetailStatusAsync()
        {
            // Lấy danh sách các OrderDetail với trạng thái "delivered"
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetQueryable()
                .Where(od => od.Status == "Delivered" && od.DeleteStatus == false)
                .ToListAsync();

            var currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            foreach (var orderDetail in orderDetails)
            {
                // Lấy ngày từ TrackingOrderD dựa trên OrderDetailId và TrackingId
                var tracking = _unitOfWork.TrackingOrderDRepository.Get()
                    .FirstOrDefault(t => t.OrderDetailId == orderDetail.OrderDetailId && t.TrackingId == 4);

                if (tracking != null)
                {
                    // So sánh ngày với ngày hiện tại
                    var daysDiff = (currentDate - tracking.Date).TotalDays;

                    if (daysDiff >= 3)
                    {
                        // Cập nhật trạng thái thành "finish"
                        orderDetail.Status = "Finish";
                        _unitOfWork.OrderDetailRepository.Update(orderDetail);

                        // Tạo tracking mới với status là finish
                        var trackingOrderD = new TrackingOrderD
                        {
                            OrderDetailId = orderDetail.OrderDetailId,
                            TrackingId = 5, // Giá trị mặc định
                            Date = currentDate,
                        };

                        _unitOfWork.TrackingOrderDRepository.Insert(trackingOrderD);

                        // Gửi thông báo và lưu vào Notification nếu trạng thái thay đổi
                        var message = $"{orderDetail.CustomerId}-{orderDetail.OrderDetailId}-{orderDetail.Status}";
                        await _hubContext.Clients.All.SendAsync("NotiCustomer", message);

                        var notification = new Notification
                        {
                            Message = message,
                            CreatedDate = currentDate,
                            IsRead = false,
                            Role = "Customer",
                            CustomerId = orderDetail.CustomerId
                        };

                        _unitOfWork.NotificationRepository.Insert(notification);
                    }
                }
            }

            // Lưu tất cả các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.SaveAsync();
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

            // Lưu trạng thái cũ để kiểm tra thay đổi
            var oldStatus = orderDetail.Status;

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
            if (request.Image != null || request.Image == null)
            {
                orderDetail.Image = request.Image; // Cho phép gán giá trị null
            }
            if (request.ConfirmationImage != null || request.ConfirmationImage == null)
            {
                orderDetail.ConfirmationImage = request.ConfirmationImage; // Cho phép gán giá trị null
            }

            if (request.DeliveryPerson != null || request.DeliveryPerson == null)
            {
                orderDetail.DeliveryPerson = request.DeliveryPerson; // Cho phép gán giá trị null
            }
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                orderDetail.Status = request.Status; // Cập nhật trạng thái
            }
            if (!string.IsNullOrWhiteSpace(request.ReceiverName)) orderDetail.ReceiverName = request.ReceiverName;
            if (!string.IsNullOrWhiteSpace(request.ReceiverPhone)) orderDetail.ReceiverPhone = request.ReceiverPhone;
            if (request.Rating.HasValue) orderDetail.Rating = request.Rating.Value; // Update Rating if provided
            if (!string.IsNullOrWhiteSpace(request.Feedback)) orderDetail.Feedback = request.Feedback; // Update Feedback if provided

            // Set DeleteStatus to false regardless of the request body
            orderDetail.DeleteStatus = request.DeleteStatus;

            // Gửi thông báo và lưu vào Notification nếu trạng thái thay đổi
            if (orderDetail.Status != oldStatus)
            {
                var message = $"{orderDetail.CustomerId}-{orderDetail.OrderDetailId}-{orderDetail.Status}";

                // Gửi thông báo đến tất cả các client đang kết nối
                await _hubContext.Clients.All.SendAsync("NotiCustomer", message);

                // Tạo Notification mới
                var notification = new Notification
                {
                    Message = message,

                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    IsRead = false, // Chưa đọc
                    Role = "Customer", // Hoặc vai trò phù hợp
                    CustomerId = orderDetail.CustomerId // Gán CustomerId
                };

                // Thêm Notification vào cơ sở dữ liệu
                _unitOfWork.NotificationRepository.Insert(notification);
            }

            // Cập nhật OrderDetail trong cơ sở dữ liệu
            _unitOfWork.OrderDetailRepository.Update(orderDetail);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // PUT: api/orderdetail/5/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrderDetail(int id)
        {
            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            var oldStatus = orderDetail.Status;

            // Set status to "cancel"
            orderDetail.Status = "Canceled";

            // Trigger notification if status has changed
            if (orderDetail.Status != oldStatus)
            {
                var message = $"{orderDetail.CustomerId}-{orderDetail.OrderDetailId}-{orderDetail.Status}";

                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", message);

                var notification = new Notification
                {
                    Message = message,
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    IsRead = false,
                    Role = "Staffs",
                    CustomerId = 0
                };

                _unitOfWork.NotificationRepository.Insert(notification);
            }

            _unitOfWork.OrderDetailRepository.Update(orderDetail);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // PUT: api/orderdetail/5/finish
        [HttpPut("{id}/finish")]
        public async Task<IActionResult> FinishOrderDetail(int id)
        {
            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            var oldStatus = orderDetail.Status;

            // Set status to "finish"
            orderDetail.Status = "Finish";

            // Trigger notification if status has changed
            if (orderDetail.Status != oldStatus)
            {
                var message = $"{orderDetail.CustomerId}-{orderDetail.OrderDetailId}-{orderDetail.Status}";

                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", message);

                var notification = new Notification
                {
                    Message = message,
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    IsRead = false,
                    Role = "Staffs",
                    CustomerId = 0
                };

                _unitOfWork.NotificationRepository.Insert(notification);
            }

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

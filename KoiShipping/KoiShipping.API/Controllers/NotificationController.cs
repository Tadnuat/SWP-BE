using KoiShipping.API.Models.NotificationModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
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
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/notification?role={role}&customerId={customerId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseNotificationModel>>> GetNotifications(int customerId)
        {
            var notifications = await Task.Run(() =>
                _unitOfWork.NotificationRepository.Get()
                .Where(n => n.CustomerId == customerId)
                .ToList());

            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found for the specified role and customer ID.");
            }

            var response = notifications.Select(notification => new ResponseNotificationModel
            {
                NotificationId = notification.NotificationId,
                Message = notification.Message,
                CreatedDate = notification.CreatedDate,
                IsRead = notification.IsRead,
                Role = notification.Role,
                CustomerId = notification.CustomerId
            });

            return Ok(response);
        }

        // PUT: api/notification/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] RequestUpdateNotificationModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notification = _unitOfWork.NotificationRepository.GetByID(id);

            if (notification == null)
            {
                return NotFound();
            }

            notification.Message = request.Message;
            notification.IsRead = request.IsRead; // Cập nhật trạng thái đọc

            _unitOfWork.NotificationRepository.Update(notification);
            await _unitOfWork.SaveAsync();

            return NoContent(); // Trả về 204 No Content nếu cập nhật thành công
        }
    }
}

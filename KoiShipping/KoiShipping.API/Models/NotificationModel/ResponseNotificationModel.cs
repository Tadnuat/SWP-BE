namespace KoiShipping.API.Models.NotificationModel
{
    public class ResponseNotificationModel
    {
        public int NotificationId { get; set; } // ID thông báo
        public string Message { get; set; } // Nội dung thông báo
        public DateTime CreatedDate { get; set; } // Ngày tạo thông báo
        public bool IsRead { get; set; } // Trạng thái đọc
        public string Role { get; set; } // Vai trò nhận thông báo
        public int CustomerId { get; set; } // Khóa ngoại tới bảng Customer
    }
}

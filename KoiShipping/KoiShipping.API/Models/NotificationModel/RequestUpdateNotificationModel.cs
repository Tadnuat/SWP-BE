namespace KoiShipping.API.Models.NotificationModel
{
    public class RequestUpdateNotificationModel
    {
        public string Message { get; set; } // Nội dung thông báo mới
        public bool IsRead { get; set; } // Trạng thái đọc (true: đã đọc, false: chưa đọc)
    }
}

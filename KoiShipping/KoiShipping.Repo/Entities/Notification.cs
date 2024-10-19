using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities
{
    public partial class Notification
    {
        public int NotificationId { get; set; } // ID tự động tăng

        public string Message { get; set; } // Nội dung thông báo

        public DateTime CreatedDate { get; set; } // Ngày tạo thông báo

        public bool IsRead { get; set; } = false; // Trạng thái đọc (false: chưa đọc, true: đã đọc)

        public string Role { get; set; } // Vai trò nhận thông báo (Manager, Sale Staff, etc.)

        public int CustomerId { get; set; } // Khóa ngoại tới bảng Customer

        // Điều hướng tới bảng Customer
        public virtual Customer Customer { get; set; } = null!;
    }
}

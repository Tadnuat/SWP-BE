﻿using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int ServiceId { get; set; }

        public decimal Weight { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string KoiStatus { get; set; } = null!;

        public string? AttachedItem { get; set; }

        public string Status { get; set; } = null!;

        public bool DeleteStatus { get; set; }

        public string ReceiverName { get; set; } = null!; // Tên người nhận

        public string ReceiverPhone { get; set; } = null!; // SĐT người nhận

        public int? Rating { get; set; } // Đánh giá từ 1 đến 5 sao (có thể null)

        public string? Feedback { get; set; } // Phản hồi từ khách hàng (có thể null)

        public virtual ICollection<AserviceOrderD> AserviceOrderDs { get; set; } = new List<AserviceOrderD>();

        public virtual Customer Customer { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;

        public virtual Service Service { get; set; } = null!;
    }
}

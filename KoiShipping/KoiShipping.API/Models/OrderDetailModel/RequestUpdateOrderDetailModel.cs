namespace KoiShipping.API.Models.OrderDetailModel
{
    public class RequestUpdateOrderDetailModel
    {
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string StartLocation { get; set; } = null!; // Vị trí bắt đầu
        public string Destination { get; set; } = null!; // Điểm đến
        public int? ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal? Weight { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? KoiStatus { get; set; }
        public string? AttachedItem { get; set; }
        public string? DeliveryPerson { get; set; }
        public string? ConfirmationImage { get; set; }
        public string? Image { get; set; }
        public string? Status { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public int? Rating { get; set; } // Rating from 1 to 5 stars (nullable)
        public string? Feedback { get; set; } // Customer feedback (nullable)
        public bool DeleteStatus { get; set; }
    }
}

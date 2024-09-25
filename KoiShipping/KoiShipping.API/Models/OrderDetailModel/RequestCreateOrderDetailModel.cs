namespace KoiShipping.API.Models.OrderDetailModel
{
    public class RequestCreateOrderDetailModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string KoiStatus { get; set; } = null!;
        public string? AttachedItem { get; set; }
        public string Status { get; set; } = null!;
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public int? Rating { get; set; } // Rating from 1 to 5 stars (nullable)
        public string? Feedback { get; set; } // Customer feedback (nullable)
    }
}

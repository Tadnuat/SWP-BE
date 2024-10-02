namespace KoiShipping.API.Models.OrderDetailModel
{
    public class ResponseOrderDetailModel
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }  // Thêm CustomerName
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string KoiStatus { get; set; } = null!;
        public string? AttachedItem { get; set; }
        public string Status { get; set; } = null!;
        public bool DeleteStatus { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

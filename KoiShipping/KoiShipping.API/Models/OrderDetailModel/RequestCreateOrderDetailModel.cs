﻿using KoiShipping.API.Models.AdvancedServiceModel;
namespace KoiShipping.API.Models.OrderDetailModel
{
    public class RequestCreateOrderDetailModel
    {
        public int CustomerId { get; set; }
        public string StartLocation { get; set; } = null!; // Vị trí bắt đầu
        public string Destination { get; set; } = null!; // Điểm đến
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string KoiStatus { get; set; } = null!;
        public string? AttachedItem { get; set; }
        public string? Image { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public List<int> SelectedAdvancedServiceIds { get; set; } // Danh sách ID của AdvancedService đã chọn
    }
}

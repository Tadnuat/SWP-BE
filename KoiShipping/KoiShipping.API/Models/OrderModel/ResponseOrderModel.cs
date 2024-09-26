using KoiShipping.API.Models.OrderDetailModel;
namespace KoiShipping.API.Models.OrderModel
{
    public class ResponseOrderModel
    {
        public int OrderId { get; set; }
        public string StartLocation { get; set; }
        public string Destination { get; set; }
        public string? TransportMethod { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string Status { get; set; }
        public decimal? TotalWeight { get; set; }
        public int? TotalKoiFish { get; set; }
        public bool DeleteStatus { get; set; }
        // Thêm danh sách OrderDetails
        public List<ResponseOrderDetailModel> OrderDetails { get; set; } = new List<ResponseOrderDetailModel>();
        // Thêm thuộc tính để chứa danh sách tên nhân viên
        public List<StaffInfo> StaffDeliveries { get; set; } = new List<StaffInfo>(); // Thay đổi ở đây
    }
    public class StaffInfo
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = null!;
    }
}

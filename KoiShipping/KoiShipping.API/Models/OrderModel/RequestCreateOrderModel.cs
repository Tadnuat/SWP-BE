namespace KoiShipping.API.Models.OrderModel
{
    public class RequestCreateOrderModel
    {
        public string StartLocation { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string? TransportMethod { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public decimal? TotalWeight { get; set; }
        public int? TotalKoiFish { get; set; }
        public List<int> StaffIds { get; set; } = new List<int>(); // Danh sách StaffId
    }
}

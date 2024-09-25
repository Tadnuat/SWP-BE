namespace KoiShipping.API.Models.OrderModel
{
    public class ResponseOrderModel
    {
        public int OrderId { get; set; }
        public string StartLocation { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string? TransportMethod { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal? TotalWeight { get; set; }
        public int? TotalKoiFish { get; set; }
        public int StaffId { get; set; }
        public bool DeleteStatus { get; set; }
    }
}

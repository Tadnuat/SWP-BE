namespace KoiShipping.API.Models.OrderModel
{
    public class RequestUpdateOrderModel
    {
        public string? StartLocation { get; set; }
        public string? Destination { get; set; }
        public string? TransportMethod { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string? Status { get; set; }
        public bool DeleteStatus { get; set; }
        public List<int> StaffIds { get; set; } = new List<int>(); // Allow updating staff
    }
}

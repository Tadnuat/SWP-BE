namespace KoiShipping.API.Models.FilterServiceModel
{
    public class RequestFilterServiceModel
    {
        public string TransportMethod { get; set; }
        public decimal Weight { get; set; }
        public string DeliveryType { get; set; } // e.g., "Fast", "Economy", "Express"
    }

}

namespace KoiShipping.API.Models.ServiceModel
{
    public class RequestUpdateServiceModel
    {
        public string? TransportMethod { get; set; }
        public string? WeightRange { get; set; }
        public decimal? FastDelivery { get; set; }
        public decimal? EconomyDelivery { get; set; }
        public decimal? ExpressDelivery { get; set; }
        public bool DeleteStatus { get; set; }
    }
}

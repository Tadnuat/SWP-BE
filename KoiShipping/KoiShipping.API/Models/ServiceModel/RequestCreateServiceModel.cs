namespace KoiShipping.API.Models.ServiceModel
{
    public class RequestCreateServiceModel
    {
        public string TransportMethod { get; set; } = null!;
        public string WeightRange { get; set; } = null!;
        public decimal FastDelivery { get; set; }
        public decimal EconomyDelivery { get; set; }
        public decimal ExpressDelivery { get; set; }
    }
}

﻿namespace KoiShipping.API.Models.ServiceModel
{
    public class ResponseServiceModel
    {
        public int ServiceId { get; set; }
        public string TransportMethod { get; set; } = null!;
        public string WeightRange { get; set; } = null!;
        public decimal FastDelivery { get; set; }
        public decimal EconomyDelivery { get; set; }
        public decimal ExpressDelivery { get; set; }
        public bool DeleteStatus { get; set; }
    }
}

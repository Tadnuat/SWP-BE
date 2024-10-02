namespace KoiShipping.API.Models.AdvancedServiceModel
{
    public class ResponseAdvancedServiceModel
    {
        public int AdvancedServiceId { get; set; }
        public string AServiceName { get; set; } = null!;
        public decimal Price { get; set; }
        public bool DeleteStatus { get; set; }
    }
}

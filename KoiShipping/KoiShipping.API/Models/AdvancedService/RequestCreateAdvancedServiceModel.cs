using System.ComponentModel.DataAnnotations;

namespace KoiShipping.API.Models.AdvancedServiceModel
{
    public class RequestCreateAdvancedServiceModel
    {
        public string AServiceName { get; set; } = null!;
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}

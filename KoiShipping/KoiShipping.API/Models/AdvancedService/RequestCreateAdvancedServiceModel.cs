using System.ComponentModel.DataAnnotations;

namespace KoiShipping.API.Models.AdvancedServiceModel
{
    public class RequestCreateAdvancedServiceModel
    {

        public int AdvancedServiceId { get; set; } // Include ID for easy updates
        public string AServiceName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}

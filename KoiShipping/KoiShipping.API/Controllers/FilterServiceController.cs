using KoiShipping.API.Models.FilterServiceModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilterServiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FilterServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ResponseFilterServiceModel>>> GetFilteredServices([FromQuery] RequestFilterServiceModel filter)
        {
            var services = await Task.Run(() => _unitOfWork.ServiceRepository.Get()
                .Where(s => !s.DeleteStatus &&
                            s.TransportMethod.Equals(filter.TransportMethod, StringComparison.OrdinalIgnoreCase) &&
                            IsWeightInRange(s.WeightRange, filter.Weight) &&
                            DeliveryTypeMatches(s, filter.DeliveryType))
                .ToList());

            // Create the response using ResponseFilterServiceModel
            var response = services.Select(service => new ResponseFilterServiceModel
            {
                ServiceId = service.ServiceId,
                Price = filter.DeliveryType.Equals("fast", StringComparison.OrdinalIgnoreCase) ? service.FastDelivery :
                        filter.DeliveryType.Equals("economy", StringComparison.OrdinalIgnoreCase) ? service.EconomyDelivery :
                        filter.DeliveryType.Equals("express", StringComparison.OrdinalIgnoreCase) ? service.ExpressDelivery :
                        0 // Default to 0 if DeliveryType is unrecognized
            }).ToList();

            return Ok(response);
        }
        // Helper method to check if weight is within the specified range
        private bool IsWeightInRange(string weightRange, decimal weight)
        {
            var ranges = weightRange.Split('-').Select(w => decimal.Parse(w.Trim())).ToArray();
            return ranges.Length == 2 && weight >= ranges[0] && weight <= ranges[1];
        }

        // Helper method to match delivery type
        private bool DeliveryTypeMatches(Service service, string deliveryType)
        {
            return deliveryType switch
            {
                "fast" => service.FastDelivery > 0,
                "economy" => service.EconomyDelivery > 0,
                "express" => service.ExpressDelivery > 0,
                _ => false,
            };
        }
    }
}
using KoiShipping.API.Models.ServiceModel;
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
    public class ServiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/service
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseServiceModel>>> GetServices()
        {
            var services = await Task.Run(() => _unitOfWork.ServiceRepository.Get().Where(s => !s.DeleteStatus).ToList());

            var response = services.Select(service => new ResponseServiceModel
            {
                ServiceId = service.ServiceId,
                TransportMethod = service.TransportMethod,
                WeightRange = service.WeightRange,
                FastDelivery = service.FastDelivery,
                EconomyDelivery = service.EconomyDelivery,
                ExpressDelivery = service.ExpressDelivery,
                DeleteStatus = service.DeleteStatus
            }).ToList();

            return Ok(response);
        }

        // GET: api/service/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseServiceModel>> GetService(int id)
        {
            var service = _unitOfWork.ServiceRepository.GetByID(id);

            if (service == null || service.DeleteStatus)
            {
                return NotFound();
            }

            var response = new ResponseServiceModel
            {
                ServiceId = service.ServiceId,
                TransportMethod = service.TransportMethod,
                WeightRange = service.WeightRange,
                FastDelivery = service.FastDelivery,
                EconomyDelivery = service.EconomyDelivery,
                ExpressDelivery = service.ExpressDelivery,
                DeleteStatus = service.DeleteStatus
            };

            return Ok(response);
        }
        
        // POST: api/service
        [HttpPost]
        public async Task<ActionResult> CreateService([FromBody] RequestCreateServiceModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // You can choose to handle the ID here if necessary, otherwise ignore it
            var service = new Service
            {
                // ID will be generated automatically by the database
                TransportMethod = request.TransportMethod,
                WeightRange = request.WeightRange,
                FastDelivery = request.FastDelivery,
                EconomyDelivery = request.EconomyDelivery,
                ExpressDelivery = request.ExpressDelivery,
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            _unitOfWork.ServiceRepository.Insert(service);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.ServiceId }, service);
        }


        // PUT: api/service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] RequestUpdateServiceModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = _unitOfWork.ServiceRepository.GetByID(id);

            if (service == null)
            {
                return NotFound();
            }

            // Update fields only if they are provided
            if (!string.IsNullOrWhiteSpace(request.TransportMethod)) service.TransportMethod = request.TransportMethod;
            if (!string.IsNullOrWhiteSpace(request.WeightRange)) service.WeightRange = request.WeightRange;
            if (request.FastDelivery.HasValue) service.FastDelivery = request.FastDelivery.Value;
            if (request.EconomyDelivery.HasValue) service.EconomyDelivery = request.EconomyDelivery.Value;
            if (request.ExpressDelivery.HasValue) service.ExpressDelivery = request.ExpressDelivery.Value;

            // Set DeleteStatus to false regardless of the request body
            service.DeleteStatus = request.DeleteStatus;

            _unitOfWork.ServiceRepository.Update(service);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = _unitOfWork.ServiceRepository.GetByID(id);

            if (service == null)
            {
                return NotFound();
            }

            _unitOfWork.ServiceRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/service/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteService(int id)
        {
            var service = _unitOfWork.ServiceRepository.GetByID(id);

            if (service == null)
            {
                return NotFound();
            }

            if (service.DeleteStatus)
            {
                return BadRequest("Service is already marked as deleted.");
            }

            _unitOfWork.ServiceRepository.SoftDelete(service);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

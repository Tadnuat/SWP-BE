using KoiShipping.API.Models.AdvancedServiceModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager")]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvancedServiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdvancedServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/advancedservice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseAdvancedServiceModel>>> GetAdvancedServices()
        {
            var services = await Task.Run(() => _unitOfWork.AdvancedServiceRepository.Get().Where(s => !s.DeleteStatus).ToList());

            var response = services.Select(service => new ResponseAdvancedServiceModel
            {
                AdvancedServiceId = service.AdvancedServiceId,
                ServiceName = service.ServiceName,
                Price = service.Price,
                DeleteStatus = service.DeleteStatus
            });

            return Ok(response);
        }

        // GET: api/advancedservice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseAdvancedServiceModel>> GetAdvancedService(int id)
        {
            var service = _unitOfWork.AdvancedServiceRepository.GetByID(id);

            if (service == null || service.DeleteStatus)
            {
                return NotFound();
            }

            var response = new ResponseAdvancedServiceModel
            {
                AdvancedServiceId = service.AdvancedServiceId,
                ServiceName = service.ServiceName,
                Price = service.Price,
                DeleteStatus = service.DeleteStatus
            };

            return Ok(response);
        }

        // POST: api/advancedservice
        [HttpPost]
        public async Task<ActionResult> CreateAdvancedService([FromBody] RequestCreateAdvancedServiceModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = new AdvancedService
            {
                AdvancedServiceId = request.AdvancedServiceId, // Include ID
                ServiceName = request.ServiceName,
                Price = request.Price,
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            _unitOfWork.AdvancedServiceRepository.Insert(service);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetAdvancedService), new { id = service.AdvancedServiceId }, service);
        }

        // PUT: api/advancedservice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvancedService(int id, [FromBody] RequestUpdateAdvancedServiceModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = _unitOfWork.AdvancedServiceRepository.GetByID(id);

            if (service == null)
            {
                return NotFound();
            }

            service.ServiceName = request.ServiceName;
            service.Price = request.Price;

            // Set DeleteStatus as per the request
            service.DeleteStatus = request.DeleteStatus;

            _unitOfWork.AdvancedServiceRepository.Update(service);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/advancedservice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvancedService(int id)
        {
            var service = _unitOfWork.AdvancedServiceRepository.GetByID(id);

            if (service == null)
            {
                return NotFound();
            }

            _unitOfWork.AdvancedServiceRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/advancedservice/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteAdvancedService(int id)
        {
            var service = _unitOfWork.AdvancedServiceRepository.GetByID(id);

            if (service == null)
            {
                return NotFound();
            }

            if (service.DeleteStatus)
            {
                return BadRequest("Service is already marked as deleted.");
            }

            _unitOfWork.AdvancedServiceRepository.SoftDelete(service);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

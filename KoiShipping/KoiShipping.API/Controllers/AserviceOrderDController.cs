using KoiShipping.API.Models.AserviceOrderDModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AserviceOrderDController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AserviceOrderDController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/aserviceorderd
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseAserviceOrderDModel>>> GetAserviceOrderDs()
        {
            var orders = await Task.Run(() => _unitOfWork.AserviceOrderDRepository.Get().ToList());

            var response = orders.Select(order => new ResponseAserviceOrderDModel
            {
                AserviceOrderId = order.AserviceOrderId,
                OrderDetailId = order.OrderDetailId,
                AdvancedServiceId = order.AdvancedServiceId,
            });

            return Ok(response);
        }

        // GET: api/aserviceorderd/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseAserviceOrderDModel>> GetAserviceOrderD(int id)
        {
            var order = _unitOfWork.AserviceOrderDRepository.GetByID(id);

            if (order == null )
            {
                return NotFound();
            }

            var response = new ResponseAserviceOrderDModel
            {
                AserviceOrderId = order.AserviceOrderId,
                OrderDetailId = order.OrderDetailId,
                AdvancedServiceId = order.AdvancedServiceId,
            };

            return Ok(response);
        }

        // POST: api/aserviceorderd
        [HttpPost]
        public async Task<ActionResult> CreateAserviceOrderD([FromBody] RequestCreateAserviceOrderDModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new AserviceOrderD
            {
                OrderDetailId = request.OrderDetailId,
                AdvancedServiceId = request.AdvancedServiceId,
            };

            _unitOfWork.AserviceOrderDRepository.Insert(order);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetAserviceOrderD), new { id = order.AserviceOrderId }, order);
        }

        // PUT: api/aserviceorderd/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAserviceOrderD(int id, [FromBody] RequestUpdateAserviceOrderDModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _unitOfWork.AserviceOrderDRepository.GetByID(id);

            if (order == null)
            {
                return NotFound();
            }

            order.OrderDetailId = request.OrderDetailId;
            order.AdvancedServiceId = request.AdvancedServiceId;


            _unitOfWork.AserviceOrderDRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/aserviceorderd/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAserviceOrderD(int id)
        {
            var order = _unitOfWork.AserviceOrderDRepository.GetByID(id);

            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.AserviceOrderDRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/aserviceorderd/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteAserviceOrderD(int id)
        {
            var order = _unitOfWork.AserviceOrderDRepository.GetByID(id);

            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.AserviceOrderDRepository.SoftDelete(order);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

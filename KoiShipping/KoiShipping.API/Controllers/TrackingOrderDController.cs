using KoiShipping.API.Models.TrackingOrderDModel;
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
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingOrderDController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrackingOrderDController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/trackingorderd
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseTrackingOrderDModel>>> GetTrackingOrderDs()
        {
            var trackingOrderDs = await Task.Run(() => _unitOfWork.TrackingOrderDRepository.Get().ToList());

            var response = trackingOrderDs.Select(orderD => new ResponseTrackingOrderDModel
            {
                TrackingOrderDId = orderD.TrackingOrderDId,
                OrderDetailId = orderD.OrderDetailId,
                TrackingId = orderD.TrackingId,
                Date = orderD.Date
            });

            return Ok(response);
        }

        // GET: api/trackingorderd/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseTrackingOrderDModel>> GetTrackingOrderD(int id)
        {
            var orderD = _unitOfWork.TrackingOrderDRepository.GetByID(id);

            if (orderD == null)
            {
                return NotFound();
            }

            var response = new ResponseTrackingOrderDModel
            {
                TrackingOrderDId = orderD.TrackingOrderDId,
                OrderDetailId = orderD.OrderDetailId,
                TrackingId = orderD.TrackingId,
                Date = orderD.Date
            };

            return Ok(response);
        }

        // GET: api/trackingorderd/byorderdetail/5
        [HttpGet("orderdetail/{orderDetailId}")]
        public async Task<ActionResult<IEnumerable<ResponseTrackingOrderDModel>>> GetTrackingOrderDsByOrderDetailId(int orderDetailId)
        {
            var trackingOrderDs = await Task.Run(() =>
                _unitOfWork.TrackingOrderDRepository.Get(t => t.OrderDetailId == orderDetailId).ToList());

            if (trackingOrderDs == null || trackingOrderDs.Count == 0)
            {
                return NotFound();
            }

            var response = trackingOrderDs.Select(orderD => new ResponseTrackingOrderDModel
            {
                TrackingOrderDId = orderD.TrackingOrderDId,
                OrderDetailId = orderD.OrderDetailId,
                TrackingId = orderD.TrackingId,
                Date = orderD.Date
            });

            return Ok(response);
        }

        // POST: api/trackingorderd
        [HttpPost]
        public async Task<ActionResult> CreateTrackingOrderD([FromBody] RequestCreateTrackingOrderDModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderD = new TrackingOrderD
            {
                OrderDetailId = request.OrderDetailId,
                TrackingId = request.TrackingId,
                Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
            };

            _unitOfWork.TrackingOrderDRepository.Insert(orderD);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetTrackingOrderD), new { id = orderD.TrackingOrderDId }, orderD);
        }

        // PUT: api/trackingorderd/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrackingOrderD(int id, [FromBody] RequestUpdateTrackingOrderDModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderD = _unitOfWork.TrackingOrderDRepository.GetByID(id);

            if (orderD == null)
            {
                return NotFound();
            }

            orderD.OrderDetailId = request.OrderDetailId;
            orderD.TrackingId = request.TrackingId;
            orderD.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            _unitOfWork.TrackingOrderDRepository.Update(orderD);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
        // DELETE: api/trackingorderd/orderdetail/{orderDetailId}/tracking/{trackingId}
        [HttpDelete("{orderDetailId}/{trackingId}")]
        public async Task<IActionResult> DeleteTrackingOrderD(int orderDetailId, int trackingId)
        {
            // Tìm đối tượng TrackingOrderD theo cả OrderDetailId và TrackingId
            var orderD = _unitOfWork.TrackingOrderDRepository.Get()
                        .SingleOrDefault(x => x.OrderDetailId == orderDetailId && x.TrackingId == trackingId);

            if (orderD == null)
            {
                return NotFound();
            }

            _unitOfWork.TrackingOrderDRepository.Delete(orderD);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

    }
}

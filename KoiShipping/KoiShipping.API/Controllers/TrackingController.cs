using KoiShipping.API.Models.TrackingModel;
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
    public class TrackingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrackingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/tracking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseTrackingModel>>> GetTrackings()
        {
            var trackings = await Task.Run(() => _unitOfWork.TrackingRepository.Get().ToList());

            var response = trackings.Select(tracking => new ResponseTrackingModel
            {
                TrackingId = tracking.TrackingId,
                TrackingName = tracking.TrackingName
            });

            return Ok(response);
        }

        // GET: api/tracking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseTrackingModel>> GetTracking(int id)
        {
            var tracking = _unitOfWork.TrackingRepository.GetByID(id);

            if (tracking == null)
            {
                return NotFound();
            }

            var response = new ResponseTrackingModel
            {
                TrackingId = tracking.TrackingId,
                TrackingName = tracking.TrackingName
            };

            return Ok(response);
        }

        // POST: api/tracking
        [HttpPost]
        public async Task<ActionResult> CreateTracking([FromBody] RequestCreateTrackingModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tracking = new Tracking
            {
                TrackingName = request.TrackingName
            };

            _unitOfWork.TrackingRepository.Insert(tracking);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetTracking), new { id = tracking.TrackingId }, tracking);
        }

        // PUT: api/tracking/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTracking(int id, [FromBody] RequestUpdateTrackingModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tracking = _unitOfWork.TrackingRepository.GetByID(id);

            if (tracking == null)
            {
                return NotFound();
            }

            tracking.TrackingName = request.TrackingName;

            _unitOfWork.TrackingRepository.Update(tracking);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/tracking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTracking(int id)
        {
            var tracking = _unitOfWork.TrackingRepository.GetByID(id);

            if (tracking == null)
            {
                return NotFound();
            }

            _unitOfWork.TrackingRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}

using DPCV_API.BAL.Services.Website.Homestays;
using DPCV_API.Models.Website.HomestayModel;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website.Homestay
{
    [Route("api/homestays")]
    [ApiController]
    public class HomestayController : ControllerBase
    {
        private readonly IHomestayService _homestayService;

        public HomestayController(IHomestayService homestayService)
        {
            _homestayService = homestayService;
        }

        // ✅ Get All Homestays
        [HttpGet]
        public async Task<ActionResult<List<HomestayDTO>>> GetAllHomestays()
        {
            var homestays = await _homestayService.GetAllHomestaysAsync();
            return Ok(homestays);
        }

        // ✅ Get Homestay by ID
        [HttpGet("{id}", Name = "GetHomestayById")]
        public async Task<ActionResult<HomestayDTO>> GetHomestayById(int id)
        {
            var homestay = await _homestayService.GetHomestayByIdAsync(id);
            if (homestay == null)
                return NotFound(new { message = "Homestay not found" });

            return Ok(homestay);
        }

        // ✅ Create Homestay
        [HttpPost]
        public async Task<ActionResult> CreateHomestay([FromBody] HomestayDTO homestay)
        {
            if (homestay == null || string.IsNullOrWhiteSpace(homestay.HomestayName) || homestay.CommitteeId == 0)
            {
                return BadRequest(new { message = "Homestay name and committee ID are required." });
            }

            bool isCreated = await _homestayService.CreateHomestayAsync(homestay);
            if (!isCreated)
            {
                return StatusCode(500, new { message = "Error creating homestay." });
            }

            return CreatedAtRoute("GetHomestayById", new { id = homestay.HomestayId }, homestay);
        }

        // ✅ Update Homestay
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateHomestay(int id, [FromBody] HomestayDTO homestay)
        {
            if (homestay == null)
            {
                return BadRequest(new { message = "Invalid homestay data." });
            }

            bool isUpdated = await _homestayService.UpdateHomestayAsync(id, homestay);
            if (!isUpdated)
            {
                return NotFound(new { message = "Homestay not found or no changes were made." });
            }

            return NoContent(); // 204 - Success but no content to return
        }
    }
}

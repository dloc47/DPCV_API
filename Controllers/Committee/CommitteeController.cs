using DPCV_API.BAL.Services.Webiste.Committees;
using DPCV_API.Models.CommitteeModel;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Committee
{
    [Route("api/committees")]
    [ApiController]
    public class CommitteeController : ControllerBase
    {
        private readonly ICommitteeService _committeeService;

        public CommitteeController(ICommitteeService committeeService)
        {
            _committeeService = committeeService;
        }

        // ✅ Get All Villages (Committees)
        [HttpGet("villages")]
        public async Task<ActionResult<List<CommitteeDTO>>> GetAllVillageNames()
        {
            var villages = await _committeeService.GetAllVillageNamesAsync();
            return Ok(villages);
        }
    }
}

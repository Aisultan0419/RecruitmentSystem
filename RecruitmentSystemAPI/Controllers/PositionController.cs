using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Services.Positions.Create;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController(IPositionService positionService) : ControllerBase
    {
        [HttpPost("create")]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] PositionCreateDTO positionCreateDTO)
        {
            var result = await positionService.Create(positionCreateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Recruiter,Admin")]
        [HttpGet]
        public async Task<ActionResult<GetPositionsDTO>> GetAll([FromQuery] GetPositionFilter getPositionFilter)
        {
             var result = await positionService.GetAll(getPositionFilter);
             return Ok(result.Value);
        }
    }
}

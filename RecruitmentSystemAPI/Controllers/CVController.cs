using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Services.CVs.Create;
using System.Security.Claims;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVController(ICVService cvService) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CVCreateDTO cvCreateDTO)
        {
            if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return Unauthorized("Invalid token");

            var result = await cvService.Create(cvCreateDTO, userId);
            return result.ToActionResult();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Services.CandidateProjects;
using RecruitmentSystemApplication.Services.CandidateProjects.Create;
using RecruitmentSystemApplication.Services.CandidateProjects.Modify;
using RecruitmentSystemApplication.Services.CandidateProjects.Tags;
using System.Security.Claims;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateProjectController(ICandidateProjectService candidateProjectService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Candidate,Admin")]
        public async Task<IActionResult> Create([FromBody] CandidateProjectCreateDTO candidateProjectCreateDTO)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid token");

            var result = await candidateProjectService.Create(userId, candidateProjectCreateDTO);
            return result.ToActionResult();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCandidateProjects([FromQuery] string? prefix)
        {
            var result = await candidateProjectService.GetCandidateProjects(prefix);
            return result.ToActionResult();
        }

        [HttpPut]
        [Authorize(Roles = "Candidate,Admin")]
        public async Task<IActionResult> Modify([FromBody] CandidateProjectModifyDTO candidateProjectModifyDTO)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid token");

            var result = await candidateProjectService.Modify(
                userId,
                User.IsInRole("Admin"),
                candidateProjectModifyDTO);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Candidate,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid token");

            var result = await candidateProjectService.Delete(userId, User.IsInRole("Admin"), id);
            return result.ToActionResult();
        }

        [HttpPost("tag")]
        [Authorize(Roles = "Candidate,Admin")]
        public async Task<IActionResult> AddTag([FromBody] CandidateProjectTagDTO candidateProjectTagDTO)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid token");

            var result = await candidateProjectService.AddTag(
                userId,
                User.IsInRole("Admin"),
                candidateProjectTagDTO);
            return result.ToActionResult();
        }

        [HttpDelete("tag")]
        [Authorize(Roles = "Candidate,Admin")]
        public async Task<IActionResult> DeleteTag([FromBody] CandidateProjectTagDTO candidateProjectTagDTO)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid token");

            var result = await candidateProjectService.DeleteTag(
                userId,
                User.IsInRole("Admin"),
                candidateProjectTagDTO);
            return result.ToActionResult();
        }

        private bool TryGetUserId(out Guid userId)
        {
            return Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
        }
    }
}

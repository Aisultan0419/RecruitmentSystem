using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Services.Tags;
using RecruitmentSystemApplication.Services.Tags.Create;
using RecruitmentSystemApplication.Services.Tags.Modify;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Recruiter,Admin")]
    public class TagController(ITagService tagService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Create([FromBody] TagCreateDTO tagCreateDTO)
        {
            var result = await tagService.Create(tagCreateDTO);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetTags([FromQuery] string? prefix)
        {
            var result = await tagService.GetTags(prefix);
            return result.ToActionResult();
        }

        [HttpPut]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Modify([FromBody] TagModifyDTO tagModifyDTO)
        {
            var result = await tagService.Modify(tagModifyDTO);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await tagService.Delete(id);
            return result.ToActionResult();
        }
    }
}

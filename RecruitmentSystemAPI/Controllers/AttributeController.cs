using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Services.Attribute.Get;
using RecruitmentSystemApplication.Services.Attribute.Candidate;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Create;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Modify;
using RecruitmentSystemApplication.Services.Attribute.Common;
using RecruitmentSystemApplication.Services.Attribute.Create;
using RecruitmentSystemApplication.Services.Attribute.Modify;
using System.Security.Claims;
using FluentResults;
namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController(IAttributeService attributeService,
        ICandidateAttributeService candidateAttributeService) : ControllerBase
    {
        [HttpGet("categories")]
        //[Authorize]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await attributeService.GetCategories();
            return Ok(categories);
        }

        [HttpPost("attribute")]
        //[Authorize] //i should change it later (roles)
        public async Task<IActionResult> CreateAttribute([FromBody] AttributeCreateDTO attributeCreationDTO) 
        {
            var result = await attributeService.CreateAttribute(attributeCreationDTO);
            return result.ToActionResult();
        }

        [HttpGet("attributes")]
        //[Authorize]
        public async Task<IActionResult> GetAttributes([FromQuery] string? prefix,
            [FromQuery] string? category, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var attributes = await attributeService.GetAttributes(prefix, category, page, pageSize);
            if (attributes.IsFailed) return BadRequest(attributes.Errors);
            return Ok(attributes.Value);
        }

        [HttpDelete("attributes")]
        //[Authorize]
        public async Task<IActionResult> DeleteAttributes([FromBody] List<Guid> attributeIds)
        {
            await attributeService.DeleteAttributes(attributeIds);
            return Ok();
        }

        [HttpPut("attribute")]
        //[Authorize]
        public async Task<IActionResult> ModifyAttribute([FromBody] AttributeModifyDTO attributeModifyDTO)
        {
            var result = await attributeService.ModifyAttribute(attributeModifyDTO);
            return result.ToActionResult();
        }

        [HttpPost("candidate-attribute")]
        [Authorize]
        public async Task<IActionResult> CreateCandidateAttribute([FromBody] CandidateAttributeCreateDTO candidateAttributeCreateDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized("Invalid token");
            }
            var result = await candidateAttributeService.CreateCandidateAttribute(userId, candidateAttributeCreateDTO);
            return result.ToActionResult();
        }

        [HttpPost("candidate-attribute-image")]
        [Authorize]
        public async Task<IActionResult> CreateCandidateImageAttribute([FromForm] CandidateAttributeImageDTO candidateAttributeImageDTO)
        {
            var file = candidateAttributeImageDTO.file;
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized("Invalid token");
            }
            using (Stream stream = file.OpenReadStream())
            {
                if (file.ContentType is null)
                {
                    return BadRequest("File content type is not set");
                }
                var result = await candidateAttributeService.CreateCandidateAttributeImage(stream, file.ContentType, userId, false, candidateAttributeImageDTO.AttributeId);

                return result.ToActionResult();
            }
        }

        [HttpPut("candidate-attribute")]
        [Authorize]
        public async Task<IActionResult> ModifyCandidateAttribute([FromBody] CandidateAttributeModifyDTO candidateAttributeModifyDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized("Invalid token");

            }
            var result = await candidateAttributeService.ModifyCandidateAttribute(userId, candidateAttributeModifyDTO);
            return result.ToActionResult();
        }
        [HttpGet("candidate-attributes")]
        public async Task<IActionResult> GetCandidateAttribute([FromQuery] string userId)
        {
            var result = await candidateAttributeService.GetCandidateAttributes(userId);
            return result.ToActionResult();
        }
        [HttpDelete("candidate-attributes")]
        public async Task<IActionResult> DeleteCandidateAttribute([FromBody] List<Guid> candidateAttributeIds)
        {
            await candidateAttributeService.DeleteCandidateAttributes(candidateAttributeIds);
            return Ok();
        }
    }
}

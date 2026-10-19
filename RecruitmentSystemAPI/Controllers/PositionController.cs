using Amazon.Util;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Services.Positions.Create;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemApplication.Services.Positions.Update;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController(IPositionService positionService) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] PositionCreateDTO positionCreateDTO)
        {
            var result = await positionService.Create(positionCreateDTO);
            return result.ToActionResult();
        }

        [HttpPut("update")]
        [Authorize(Roles = "Recruiter, Admin")]
        public async Task<IActionResult> Update([FromBody] PositionUpdateDTO positionUpdateDTO)
        {
            var result = await positionService.Update(positionUpdateDTO);
            return result.ToActionResult();
        }
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpGet]
        public async Task<ActionResult<GetPositionsDTO>> GetAll([FromQuery] GetPositionFilter getPositionFilter)
        {
            var result = await positionService.GetAll(getPositionFilter);
            return Ok(result.Value);
        }

        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPost("attribute")]
        public async Task<IActionResult> AddAttribute([FromQuery] Guid AttributeId, [FromQuery] Guid PositionId)
        {
            var result = await positionService.AddAttribute(AttributeId, PositionId);
            return result.ToActionResult();
        }
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpDelete("attribute")]
        public async Task<IActionResult> DeleteAttribute([FromQuery] Guid AttributeId, [FromQuery] Guid PositionId)
        {
            var result = await positionService.Delete(AttributeId, PositionId);
            return result.ToActionResult();
        }
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPost("access-rule")]
        public async Task<IActionResult> AddAccessRule(AccessRuleDTO accessRuleDTO)
        {
            var result = await positionService.AddAccessRule(accessRuleDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut("access-rule")]
        public async Task<IActionResult> ModifyAccessRule(Guid positionAccessRuleId, string value, string? secondValue, ComparisonOperator comparisonOperator)
        {
            var result = await positionService.ModifyAccessRule(positionAccessRuleId, value, secondValue, comparisonOperator);
            return result.ToActionResult();
        }
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpDelete("access-rule")]
        public async Task<IActionResult> DeleteAccessRule(Guid positionAccessRuleId)
        {
            var result = await positionService.DeleteAccessRule(positionAccessRuleId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("positionAccessRules")]
        public async Task<IActionResult> GetAccessRules([FromQuery] Guid positionId)
        {
            var result = await positionService.GetAccessRules(positionId);
            return result.ToActionResult();
        }
    }
}

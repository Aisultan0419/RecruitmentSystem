using FluentResults;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemApplication.Services.Positions.Update;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemApplication.Services.Positions.Create
{
    public interface IPositionService
    {
        Task<Result<Guid>> Create(PositionCreateDTO positionCreateDTO);
        Task<Result<GetPositionsDTO>> GetAll(GetPositionFilter getPositionFilter);
        Task<Result> AddAttribute(Guid attributeId, Guid positionId);
        Task<Result> AddAccessRule(AccessRuleDTO accessRuleDTO);
        Task<Result> ModifyAccessRule(Guid positionAccessRuleId, string value, string? secondValue, ComparisonOperator comparisonOperator);
        Task<Result> DeleteAccessRule(Guid Id);
        Task<Result<int>> Update(PositionUpdateDTO positionUpdateDTO);
        Task<Result> Delete(Guid attributeId, Guid positionId);
        Task<Result<List<PositionAccessRule>>> GetAccessRules(Guid positionId);
    }
}

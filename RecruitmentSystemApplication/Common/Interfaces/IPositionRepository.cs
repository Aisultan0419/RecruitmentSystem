
using FluentResults;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IPositionRepository
    {
        Task<GetPositionsDTO> GetAll(GetPositionFilter getPositionFilter);
        Task<Result<PositionAttribute>> AddAttribute(Guid attributeId, Guid positionId);
        Task<Position?> GetPosition(Guid id);
        Task<PositionAccessRule?> GetAccessRule(Guid id);
        Task<Result> DeleteAttribute(Guid positionId, Guid attributeId);
        Task<Result<List<PositionAccessRule>>> GetAccessRules(Guid positionId);
    }
}

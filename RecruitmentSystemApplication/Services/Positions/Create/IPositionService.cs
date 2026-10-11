using FluentResults;
using RecruitmentSystemApplication.Services.Positions.Get;

namespace RecruitmentSystemApplication.Services.Positions.Create
{
    public interface IPositionService
    {
        Task<Result> Create(PositionCreateDTO positionCreateDTO);
        Task<Result<GetPositionsDTO>> GetAll(GetPositionFilter getPositionFilter);
    }
}

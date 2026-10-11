
using RecruitmentSystemApplication.Services.Positions.Get;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IPositionRepository
    {
        Task<GetPositionsDTO> GetAll(GetPositionFilter getPositionFilter);
    }
}

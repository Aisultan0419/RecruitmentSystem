using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Positions.Create;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Positions
{
    public class PositionService(IBaseRepository baseRepository
        ,IPositionRepository positionRepository) : IPositionService
    {
        public async Task<Result> Create(PositionCreateDTO positionCreateDTO)
        {
            Position position = new Position() 
            {
                Id = Guid.NewGuid(),
                Title = positionCreateDTO.Title,
                Description = positionCreateDTO.Description,
                MaxProjectsCount = positionCreateDTO.MaxProjectsCount,
                IsPublic = false,
                CreatedAt = DateTime.UtcNow,
            };

            await baseRepository.AddItem(position);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result<GetPositionsDTO>> GetAll(GetPositionFilter getPositionFilter)
        {
            var result = await positionRepository.GetAll(getPositionFilter);
            return Result.Ok(result);
        }
    }
}

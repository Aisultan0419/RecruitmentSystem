using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.CVs.Create;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.CVs
{
    public class CVService(
        IBaseRepository baseRepository,
        IPositionRepository positionRepository,
        IUserRepository userRepository) : ICVService
    {
        public async Task<Result<Guid>> Create(CVCreateDTO cvCreateDTO, Guid userId)
        {
            var position = await positionRepository.GetPosition(cvCreateDTO.PositionId);
            if (position is null)
                return Result.Fail($"Position with id - {cvCreateDTO.PositionId} was not found");

            var user = await userRepository.FindUserById(userId);
            if (user is null)
                return Result.Fail($"User with id - {userId} was not found");

            var cv = new CV
            {
                Id = Guid.NewGuid(),
                CVState = CVState.Draft,
                PositionId = cvCreateDTO.PositionId,
                UserId = userId
            };

            await baseRepository.AddItem(cv);
            await baseRepository.SaveChanges();
            return Result.Ok(cv.Id);
        }
    }
}

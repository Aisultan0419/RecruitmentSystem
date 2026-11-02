using FluentResults;

namespace RecruitmentSystemApplication.Services.CVs.Create
{
    public interface ICVService
    {
        Task<Result<Guid>> Create(CVCreateDTO cvCreateDTO, Guid userId);
    }
}

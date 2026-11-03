using FluentResults;
using RecruitmentSystemApplication.Services.CandidateProjects.Create;
using RecruitmentSystemApplication.Services.CandidateProjects.Get;
using RecruitmentSystemApplication.Services.CandidateProjects.Modify;
using RecruitmentSystemApplication.Services.CandidateProjects.Tags;

namespace RecruitmentSystemApplication.Services.CandidateProjects
{
    public interface ICandidateProjectService
    {
        Task<Result<Guid>> Create(Guid userId, CandidateProjectCreateDTO candidateProjectCreateDTO);
        Task<Result<List<CandidateProjectDTO>>> GetCandidateProjects(string? prefix);
        Task<Result> Modify(Guid userId, bool isAdmin, CandidateProjectModifyDTO candidateProjectModifyDTO);
        Task<Result> Delete(Guid userId, bool isAdmin, Guid candidateProjectId);
        Task<Result> AddTag(Guid userId, bool isAdmin, CandidateProjectTagDTO candidateProjectTagDTO);
        Task<Result> DeleteTag(Guid userId, bool isAdmin, CandidateProjectTagDTO candidateProjectTagDTO);
    }
}

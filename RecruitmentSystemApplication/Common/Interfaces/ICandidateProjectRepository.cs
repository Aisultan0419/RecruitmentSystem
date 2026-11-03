using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface ICandidateProjectRepository
    {
        Task<List<CandidateProject>> GetCandidateProjects(string? prefix);
        Task<CandidateProject?> GetCandidateProject(Guid id);
    }
}

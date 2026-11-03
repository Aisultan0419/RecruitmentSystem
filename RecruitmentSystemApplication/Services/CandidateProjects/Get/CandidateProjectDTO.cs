using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.CandidateProjects.Get
{
    public record CandidateProjectDTO(
        Guid Id,
        Guid UserId,
        string Name,
        DateTime? StartDate,
        DateTime? EndDate,
        string? Description,
        List<Tag> Tags);
}

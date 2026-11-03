namespace RecruitmentSystemApplication.Services.CandidateProjects.Create
{
    public record CandidateProjectCreateDTO(
        string Name,
        DateTime? StartDate,
        DateTime? EndDate,
        string? Description);
}

namespace RecruitmentSystemApplication.Services.CandidateProjects.Modify
{
    public record CandidateProjectModifyDTO(
        Guid Id,
        string Name,
        DateTime? StartDate,
        DateTime? EndDate,
        string? Description);
}

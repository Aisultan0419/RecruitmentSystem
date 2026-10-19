namespace RecruitmentSystemApplication.Services.Positions.Update
{
    public record PositionUpdateDTO(Guid Id, string Title, string? Description
        , int MaxProjectsCount
        , int Version);
}

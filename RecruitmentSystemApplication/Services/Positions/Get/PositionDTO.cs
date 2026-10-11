
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Positions.Get
{
    public record PositionDTO(string Title, string? Description
        ,int MaxProjectsCount, bool IsDraft
        ,bool IsPublic, int Version, DateTime CreatedAt, DateTime? UpdatedAt, ICollection<Tag> Tags);
}

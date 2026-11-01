using Microsoft.AspNetCore.Mvc;

namespace RecruitmentSystemApplication.Services.Positions.Get
{
    public record GetPositionFilter(string? Title, string? Description, bool? IsDraft
        ,int Page, int PageSize, bool NewestFirst = true);
}

using Microsoft.AspNetCore.Mvc;

namespace RecruitmentSystemApplication.Services.Positions.Get
{
    public record GetPositionFilter(string? title, string? description, bool? isDraft, bool newestFirst = true);
}

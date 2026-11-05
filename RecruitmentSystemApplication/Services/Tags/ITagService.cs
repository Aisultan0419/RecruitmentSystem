using FluentResults;
using RecruitmentSystemApplication.Services.Tags.Create;
using RecruitmentSystemApplication.Services.Tags.Modify;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Tags
{
    public interface ITagService
    {
        Task<Result<Guid>> Create(TagCreateDTO tagCreateDTO);
        Task<Result<List<Tag>>> GetTags(string? prefix);
        Task<Result<List<Tag>>> GetTagsByPosition(Guid positionId);
        Task<Result> AddTagToPosition(Guid positionId, Guid tagId);
        Task<Result> RemoveTagFromPosition(Guid positionId, Guid tagId);
        Task<Result> Modify(TagModifyDTO tagModifyDTO);
        Task<Result> Delete(Guid id);
    }
}

using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Tags.Create;
using RecruitmentSystemApplication.Services.Tags.Modify;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Tags
{
    public class TagService(
        ITagRepository tagRepository,
        IPositionRepository positionRepository,
        IBaseRepository baseRepository) : ITagService
    {
        public async Task<Result<Guid>> Create(TagCreateDTO tagCreateDTO)
        {
            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = tagCreateDTO.Name
            };

            await baseRepository.AddItem(tag);
            await baseRepository.SaveChanges();
            return Result.Ok(tag.Id);
        }

        public async Task<Result<List<Tag>>> GetTags(string? prefix)
        {
            var tags = await tagRepository.GetTagsByPrefix(prefix);
            return Result.Ok(tags);
        }

        public async Task<Result<List<Tag>>> GetTagsByPosition(Guid positionId)
        {
            var position = await positionRepository.GetPositionWithTags(positionId);
            if (position is null)
                return Result.Fail<List<Tag>>(new Error($"Position with id - {positionId} was not found").WithMetadata("StatusCode", 404));

            return Result.Ok(position.Tags.ToList());
        }

        public async Task<Result> AddTagToPosition(Guid positionId, Guid tagId)
        {
            var position = await positionRepository.GetPositionWithTags(positionId);
            if (position is null)
                return Result.Fail(new Error($"Position with id - {positionId} was not found").WithMetadata("StatusCode", 404));

            var tag = await tagRepository.GetTag(tagId);
            if (tag is null)
                return Result.Fail(new Error($"Tag with id - {tagId} was not found").WithMetadata("StatusCode", 404));

            if (position.Tags.Any(existingTag => existingTag.Id == tagId))
                return Result.Fail("Tag is already assigned to this position");

            position.Tags.Add(tag);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> RemoveTagFromPosition(Guid positionId, Guid tagId)
        {
            var position = await positionRepository.GetPositionWithTags(positionId);
            if (position is null)
                return Result.Fail(new Error($"Position with id - {positionId} was not found").WithMetadata("StatusCode", 404));

            var tag = position.Tags.FirstOrDefault(existingTag => existingTag.Id == tagId);
            if (tag is null)
                return Result.Fail(new Error($"Tag with id - {tagId} is not assigned to position with id - {positionId}")
                    .WithMetadata("StatusCode", 404));

            position.Tags.Remove(tag);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> Modify(TagModifyDTO tagModifyDTO)
        {
            var tag = await tagRepository.GetTag(tagModifyDTO.Id);
            if (tag is null)
                return Result.Fail($"Tag with id - {tagModifyDTO.Id} was not found");

            tag.Name = tagModifyDTO.Name;
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> Delete(Guid id)
        {
            var tag = await tagRepository.GetTag(id);
            if (tag is null)
                return Result.Fail($"Tag with id - {id} was not found");

            await baseRepository.DeleteItems<Tag>(new List<Guid> { id });
            return Result.Ok();
        }
    }
}

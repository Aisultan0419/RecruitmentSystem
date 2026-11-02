using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Tags.Create;
using RecruitmentSystemApplication.Services.Tags.Modify;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Tags
{
    public class TagService(
        ITagRepository tagRepository,
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

        public async Task<Result<List<Tag>>> GetTags(string prefix)
        {
            var tags = await tagRepository.GetTagsByPrefix(prefix);
            return Result.Ok(tags);
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

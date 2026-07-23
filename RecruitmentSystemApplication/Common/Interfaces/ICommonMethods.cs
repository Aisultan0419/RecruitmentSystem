
using FluentResults;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface ICommonMethods
    {
        Task<Result<string>> UploadImageAsync(Stream fileStream, string contentType, string userId, bool isAvatar
            ,AttributeDefinition? attributeDefintion);
    }
}

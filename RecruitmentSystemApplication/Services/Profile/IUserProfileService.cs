using FluentResults;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemApplication.Services.Profile
{
    public interface IUserProfileService
    {
        Task<UserProfileDTO> Get(string userId);
        Task<Result> Modify(string userId, UserProfileDTO userProfileDTO);
        Task<Result<string>> UploadAvatarAsync(Stream fileStream, string contentType, string userId);
    }
}

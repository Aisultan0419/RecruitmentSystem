using FluentResults;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemApplication.Services.Profile
{
    public interface IUserProfileService
    {
        Task<Result<UserProfileDTO>> Get(string userId);
        Task<Result> Modify(string userId, UserProfileDTO userProfileDTO);
    }
}

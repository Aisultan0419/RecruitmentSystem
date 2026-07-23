using FluentResults;
using RecruitmentSystemApplication.Services.Profile;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindUser(string email);
        Task<UserProfile?> FindUserProfile(Guid id);
        Task<Result> UpdateProfile(string userId, UserProfileDTO userProfileDTO);
    }
}

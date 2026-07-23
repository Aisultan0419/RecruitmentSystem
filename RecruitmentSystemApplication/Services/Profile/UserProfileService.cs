using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;
using FluentResults;
namespace RecruitmentSystemApplication.Services.Profile
{
    public class UserProfileService(
         IUserRepository userRepository
        ,IBaseRepository baseRepository) : IUserProfileService
    {
        public async Task<Result<UserProfileDTO>> Get(string userId)
        {
            Guid.TryParse(userId, out Guid result);
            UserProfile? fullUserProfile = await userRepository.FindUserProfile(result);
            if (fullUserProfile is null) return Result.Fail(new Error("Unauthorized user").WithMetadata("StatusCode", "401"));
            var userProfile = new UserProfileDTO(FirstName: fullUserProfile.FirstName ?? string.Empty,
                LastName: fullUserProfile.LastName ?? string.Empty,
                Location: fullUserProfile.Location ?? string.Empty,
                PhotoUrl: fullUserProfile.PhotoUrl ?? string.Empty,
                Version: fullUserProfile.Version);
            return userProfile;
        }
        public async Task<Result> Modify(string userId, UserProfileDTO userProfileDTO)
        {
            var updateResult = await userRepository.UpdateProfile(userId, userProfileDTO);
            if (updateResult.IsFailed) return updateResult;
            await baseRepository.SaveChanges();
            return Result.Ok();
        }
    }
}

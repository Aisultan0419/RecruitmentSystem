using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;
using FluentResults;
namespace RecruitmentSystemApplication.Services.Profile
{
    public class UserProfileService(
         IUserRepository userRepository
        ,IBaseRepository baseRepository
        ,IImageService imageService) : IUserProfileService
    {
        public async Task<UserProfileDTO> Get(string userId)
        {
            Guid.TryParse(userId, out Guid result);
            UserProfile fullUserProfile = await userRepository.FindUserProfile(result);
            var userProfile = new UserProfileDTO(FirstName: fullUserProfile.FirstName ?? string.Empty,
                LastName: fullUserProfile.LastName ?? string.Empty,
                Location: fullUserProfile.Location ?? string.Empty,
                PhotoUrl: fullUserProfile.PhotoUrl ?? string.Empty);
            return userProfile;
        }
        public async Task<Result> Modify(string userId, UserProfileDTO userProfileDTO)
        {
            Guid.TryParse(userId, out Guid result);
            var userProfile = await userRepository.FindUserProfile(result);

            userProfile.FirstName = string.IsNullOrWhiteSpace(userProfileDTO.FirstName)
                ? userProfile.FirstName
                : userProfileDTO.FirstName;

            userProfile.LastName = string.IsNullOrWhiteSpace(userProfileDTO.LastName)
                ? userProfile.LastName
                : userProfileDTO.LastName;

            userProfile.Location = string.IsNullOrWhiteSpace(userProfileDTO.Location)
                ? userProfile.Location
                : userProfileDTO.Location;

            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result<string>> UploadAvatarAsync(Stream fileStream, string contentType, string userId)
        {
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedContentTypes.Contains(contentType))
            {
                return Result.Fail("You can only insert PNG/JPEG/WEBP");
            }

            string imageUrl = await imageService.SaveImage(fileStream);

            Guid.TryParse(userId, out Guid result);
            var userProfile = await userRepository.FindUserProfile(result);

            userProfile.PhotoUrl = imageUrl;

            await baseRepository.SaveChanges();

            return Result.Ok(imageUrl);
        }
    }
}

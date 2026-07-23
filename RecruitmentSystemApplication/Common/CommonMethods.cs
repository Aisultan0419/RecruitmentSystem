using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Profile;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common
{
    public class CommonMethods(IImageService imageService
        ,IBaseRepository baseRepository
        ,IUserRepository userRepository) : ICommonMethods
    {
        public async Task<Result<string>> UploadImageAsync(Stream fileStream, string contentType, string userId, bool isAvatar
            ,AttributeDefinition? attributeDefintion)
        {
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedContentTypes.Contains(contentType))
            {
                return Result.Fail("You can only insert PNG/JPEG/WEBP");
            }

            string imageUrl = await imageService.SaveImage(fileStream, isAvatar);

            Guid.TryParse(userId, out Guid result);
            UserProfile? userProfile = await userRepository.FindUserProfile(result);
            if (userProfile is null) return Result.Fail(new Error("Unauthorized user").WithMetadata("StatusCode", 401));
            if(isAvatar) userProfile.PhotoUrl = imageUrl;
            else
            {
                CandidateAttributeValue candidateAttributeValue = new() {
                    Id = Guid.NewGuid(),
                    Value = imageUrl,
                    UserProfile = userProfile,
                    UserProfileId = userProfile.Id,
                    AttributeId = attributeDefintion!.Id,
                    AttributeDefinition = attributeDefintion,
                    UpdatedAt = DateTime.UtcNow
                };
                await baseRepository.AddItem(candidateAttributeValue);
            }
            await baseRepository.SaveChanges();

            return Result.Ok(imageUrl);
        }
    }
}

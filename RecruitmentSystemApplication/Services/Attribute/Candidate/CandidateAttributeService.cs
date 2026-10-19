using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Create;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Get;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Modify;
using RecruitmentSystemApplication.Services.Profile;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;
using System.Globalization;
using System.Net.Mime;

namespace RecruitmentSystemApplication.Services.Attribute.Candidate
{
    public class CandidateAttributeService(
        IUserRepository userRepository
        , IAttributeRepository attributeRepository
        , IBaseRepository baseRepository
        , ICommonMethods commonMethods
        ,AttributeValueValidator attributeValueValidator,
        IImageService imageService) : ICandidateAttributeService
    {
        private record CandidateAttributeData(UserProfile? UserProfile, AttributeDefinition? AttributeDefinition);

        private async Task<Result<CandidateAttributeData>> GetCandidateAttributeData(string userId, string? attributeId)
        {
            if (Guid.TryParse(userId, out Guid parsedUserId) is false)
            {
                return Result.Fail(new Error("Unauthorized").WithMetadata("StatusCode", 401));
            }

            UserProfile? userProfile = await userRepository.FindUserProfile(parsedUserId);
            if (userProfile is null) return Result.Fail(new Error("Unauthorized").WithMetadata("StatusCode", 401));
            AttributeDefinition? attribute = null;
            if (attributeId is not null)
            {
                attribute = await attributeRepository.GetAttribute(attributeId);
                if (attribute is null) return Result.Fail(new Error("Attribute not found").WithMetadata("StatusCode", 400));
            }

            return new CandidateAttributeData(userProfile, attribute);
        }

        public async Task<Result> ModifyCandidateAttributeImage(Stream fileStream, string contentType, string userId, CandidateAttributeImageDTO candidateAttributeImageDTO)
        {
            Result<CandidateAttributeData> candidateAttributeDataResult = await GetCandidateAttributeData(userId, candidateAttributeImageDTO.AttributeId);
            if (candidateAttributeDataResult.IsFailed) return candidateAttributeDataResult.ToResult();
            var (userProfile, attribute) = candidateAttributeDataResult.Value;
            if (attribute!.DataType != DataType.Image)
            {
                return Result.Fail(new Error("You can insert image only with image data type attribute").WithMetadata("StatusCode", 400));
            }
            var candidateAttributeValue = await attributeRepository.GetCandidateAttributeValue(userProfile!, attribute.Id);
            if (candidateAttributeValue is null) return Result.Fail(new Error("Attribute value was not found").WithMetadata("StatusCode", 404));

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedContentTypes.Contains(contentType))
            {
                return Result.Fail("You can only insert PNG/JPEG/WEBP");
            }

            var imageUrl = await imageService.SaveImage(fileStream, false);
            await imageService.DeleteImage(imageUrl);
            candidateAttributeValue.Value = imageUrl;
            candidateAttributeValue.UpdatedAt = DateTime.UtcNow;
            candidateAttributeValue.Version += 1;
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> CreateCandidateAttribute(string userId, CandidateAttributeCreateDTO candidateAttributeCreateDTO)
        {
            Result<CandidateAttributeData> candidateAttributeDataResult = await GetCandidateAttributeData(userId, candidateAttributeCreateDTO.AttributeId);
            if (candidateAttributeDataResult.IsFailed) return candidateAttributeDataResult.ToResult();
            var (userProfile, attribute) = candidateAttributeDataResult.Value;
            var validationResult = await attributeValueValidator.ValidateValue(candidateAttributeCreateDTO.Value
                ,attribute!.DataType, attribute!.Id.ToString());
            if (validationResult.IsFailed) return validationResult.ToResult();

            CandidateAttributeValue candidateAttributeValue = new()
            {
                Id = Guid.NewGuid(),
                Value = candidateAttributeCreateDTO.Value,
                UserProfile = userProfile!,
                UserProfileId = userProfile!.Id,
                AttributeId = attribute.Id,
                AttributeDefinition = attribute,
                UpdatedAt = DateTime.UtcNow
            };
            await baseRepository.AddItem(candidateAttributeValue);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> ModifyCandidateAttribute(string userId, CandidateAttributeModifyDTO candidateAttributeModifyDTO)
        {
            Result<CandidateAttributeData> candidateAttributeDataResult = await GetCandidateAttributeData(userId, candidateAttributeModifyDTO.AttributeId);
            if (candidateAttributeDataResult.IsFailed) return candidateAttributeDataResult.ToResult();
            var (userProfile, attribute) = candidateAttributeDataResult.Value;
            var validationResult = await attributeValueValidator.ValidateValue(candidateAttributeModifyDTO.Value
                , attribute!.DataType, attribute!.Id.ToString());
            if (validationResult.IsFailed) return validationResult.ToResult();

            var existingCandidateValue = userProfile!.CandidateAttributeValues.FirstOrDefault(v => v.AttributeId == attribute.Id);
            if (existingCandidateValue is null)
            {
                return Result.Fail(new Error("Attribute value not found to modify").WithMetadata("StatusCode", 404));
            }
            if (existingCandidateValue.Version != candidateAttributeModifyDTO.Version)
            {
                return Result.Fail(new Error("The entity you are trying to update has been modified by another user")
                    .WithMetadata("StatusCode", 409));
            }
            existingCandidateValue.Value = candidateAttributeModifyDTO.Value;
            existingCandidateValue.UpdatedAt = DateTime.UtcNow;
            ++existingCandidateValue.Version;
            await baseRepository.SaveChanges();
            return Result.Ok();
        }
        public async Task<Result<List<CandidateAttributesDTO>>> GetCandidateAttributes(string userId)
        {
            if (Guid.TryParse(userId, out Guid parsedUserId) is false)
            {
                return Result.Fail(new Error("Candidate id format is invalid").WithMetadata("StatusCode", 400));
            }
            Result<CandidateAttributeData> candidateAttributeDataResult = await GetCandidateAttributeData(userId, null);
            if (candidateAttributeDataResult.IsFailed) return candidateAttributeDataResult.ToResult();
            var userProfile = candidateAttributeDataResult.Value.UserProfile;
            List<CandidateAttributesDTO> candidateAttributesDTOs = userProfile!.CandidateAttributeValues.Select(a => new CandidateAttributesDTO(a.Id,
                a.Value, a.AttributeId, a.AttributeDefinition.Name, a.AttributeDefinition.DataType, a.UserProfileId, a.Version, a.UpdatedAt)).ToList();
            return Result.Ok(candidateAttributesDTOs);
        }

        public async Task DeleteCandidateAttributes(List<Guid> candidateAttributeIds)
        {
            await baseRepository.DeleteItems<CandidateAttributeValue>(candidateAttributeIds);
        }

        public async Task<Result> CreateCandidateAttributeImage(Stream fileStream, string contentType, string userId, bool isAvatar
            , string attributeId)
        {
            AttributeDefinition? attribute = await attributeRepository.GetAttribute(attributeId);
            if(attribute is null) return Result.Fail(new Error("Attribute not found").WithMetadata("StatusCode", 400));

            if (attribute.DataType != DataType.Image)
            {
                return Result.Fail(new Error("You can insert image only with image data type attribute").WithMetadata("StatusCode", 400));
            }

            var result = await commonMethods.UploadImageAsync(fileStream, contentType, userId, isAvatar, attribute);

            return result.ToResult();
        }

        
    }
}

using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Create;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Get;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Modify;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;
using System.Globalization;

namespace RecruitmentSystemApplication.Services.Attribute.Candidate
{
    public class CandidateAttributeService(
        IUserRepository userRepository
        , IAttributeRepository attributeRepository
        , IBaseRepository baseRepository
        , ICommonMethods commonMethods) : ICandidateAttributeService
    {
        private const int STRING_MAX_SIZE = 255;
        private const int TEXT_MAX_SIZE = 50_000;
        private const decimal MIN_VALUE = -999999999.99m;
        private const decimal MAX_VALUE = +999999999.99m;
        readonly string[] allowedDateFormats = { "yyyy-MM-dd" };

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
        public async Task<Result> CreateCandidateAttribute(string userId, CandidateAttributeCreateDTO candidateAttributeCreateDTO)
        {
            Result<CandidateAttributeData> candidateAttributeDataResult = await GetCandidateAttributeData(userId, candidateAttributeCreateDTO.AttributeId);
            if (candidateAttributeDataResult.IsFailed) return candidateAttributeDataResult.ToResult();
            var (userProfile, attribute) = candidateAttributeDataResult.Value;
            var validationResult = await ValidateValue(candidateAttributeCreateDTO.Value
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
            var validationResult = await ValidateValue(candidateAttributeModifyDTO.Value
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
                a.Value, a.AttributeId, a.UserProfileId, a.Version, a.UpdatedAt)).ToList();
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

        private async Task<Result<bool>> ValidateValue(string value, DataType dataType, string attributeId)
        {
            if (dataType is DataType.String && value.Length > STRING_MAX_SIZE)
            {
                return Result.Fail(new Error($"The value is too long. Maximum allowed length is {STRING_MAX_SIZE} characters.").WithMetadata("StatusCode", 400));
            }
            else if (dataType is DataType.Text && value.Length > TEXT_MAX_SIZE)
            {
                return Result.Fail(new Error($"The value is too long. Maximum allowed length is {TEXT_MAX_SIZE} characters.").WithMetadata("StatusCode", 400));
            }
            else if (dataType is DataType.Numeric)
            {
                if (!decimal.TryParse(value, CultureInfo.InvariantCulture, out decimal numericValue))
                {
                    return Result.Fail(new Error("The value is invalid for numeric data type").WithMetadata("StatusCode", 400));
                }
                else if (numericValue < MIN_VALUE || numericValue > MAX_VALUE)
                {
                    return Result.Fail(new Error("The value size is invalid").WithMetadata("StatusCode", 400));
                }
            }
            else if (dataType is DataType.Date && !DateTime.TryParseExact(value, allowedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return Result.Fail(new Error("Invalid date format").WithMetadata("StatusCode", 400));
            }
            else if (dataType is DataType.Period)
            {
                Result periodError = Result.Fail(new Error("Invalid period format").WithMetadata("StatusCode", 400));
                var parts = value.Split("/");
                if (parts.Length != 2)
                {
                    return periodError;
                }
                else if (!DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start) ||
                          !DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end))
                {
                    return periodError;
                }
                else if (end < start)
                {
                    return periodError;
                }
            }
            else if (dataType is DataType.Boolean)
            {
                string parsedBoolean = value.Replace(" ", "").ToLower();
                if (parsedBoolean != "false" && parsedBoolean != "true")
                {
                    return Result.Fail(new Error("Invalid boolean format").WithMetadata("StatusCode", 400));
                }
            }
            else if (dataType is DataType.OneOfMany)
            {
                var attribute = await attributeRepository.GetAttribute(attributeId);
                if(attribute!.AttributeOptions.Any(a => a.Value.ToLower() == value.ToLower()) is false) return Result.Fail(new Error("Attribute option was not found").WithMetadata("StatusCode", 400)); 
            }
            return true;
        }
    }
}

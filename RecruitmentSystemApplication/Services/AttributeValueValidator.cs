using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using System.Globalization;
using RecruitmentSystemDomain.Enums;
namespace RecruitmentSystemApplication.Services
{
    public class AttributeValueValidator(IAttributeRepository attributeRepository)
    {
        private const int STRING_MAX_SIZE = 255;
        private const int TEXT_MAX_SIZE = 50_000;
        private const decimal MIN_VALUE = -999999999.99m;
        private const decimal MAX_VALUE = +999999999.99m;
        readonly string[] allowedDateFormats = { "yyyy-MM-dd" };
        public async Task<Result<bool>> ValidateValue(string value, DataType dataType, string attributeId)
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
                if (attribute!.AttributeOptions.Any(a => a.Value.ToLower() == value.ToLower()) is false) return Result.Fail(new Error("Attribute option was not found").WithMetadata("StatusCode", 400));
            }
            return true;
        }
    }
}

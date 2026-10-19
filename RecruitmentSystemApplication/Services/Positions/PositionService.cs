using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Positions.Create;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemApplication.Services.Positions.Update;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Positions
{
    public class PositionService(IBaseRepository baseRepository
        ,IPositionRepository positionRepository
        ,IAttributeRepository attributeRepository
        ,AttributeValueValidator attributeValueValidator) : IPositionService
    {
        public async Task<Result<Guid>> Create(PositionCreateDTO positionCreateDTO)
        {
            Position position = new Position() 
            {
                Id = Guid.NewGuid(),
                Title = positionCreateDTO.Title,
                Description = positionCreateDTO.Description,
                MaxProjectsCount = positionCreateDTO.MaxProjectsCount,
                IsPublic = false,
                CreatedAt = DateTime.UtcNow,
            };

            await baseRepository.AddItem(position);
            await baseRepository.SaveChanges();
            return Result.Ok(position.Id);
        }

        public async Task<Result<int>> Update(PositionUpdateDTO positionUpdateDTO)
        {
            var position = await positionRepository.GetPosition(positionUpdateDTO.Id);
            if (position is null)
                return Result.Fail($"Position with id - {positionUpdateDTO.Id} was not found");

            position.Title = positionUpdateDTO.Title;
            position.Description = positionUpdateDTO.Description;
            position.MaxProjectsCount = positionUpdateDTO.MaxProjectsCount;
            position.Version = positionUpdateDTO.Version + 1;

            await baseRepository.SaveChanges();
            return Result.Ok(position.Version);
        }

        public async Task<Result> Delete(Guid attributeId, Guid positionId)
        {
            var result = await positionRepository.DeleteAttribute(positionId, attributeId);
            return result;
        }

        public async Task<Result<GetPositionsDTO>> GetAll(GetPositionFilter getPositionFilter)
        {
            var result = await positionRepository.GetAll(getPositionFilter);
            return Result.Ok(result);
        }
        
        public async Task<Result> AddAttribute(Guid attributeId, Guid positionId)
        {
            var result = await positionRepository.AddAttribute(attributeId, positionId);
            await baseRepository.AddItem(result.Value);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }
        
        public async Task<Result> AddAccessRule(AccessRuleDTO accessRuleDTO)
        {
            var validationResult = await ValidateAccessRule(accessRuleDTO);
            if (validationResult.IsFailed) return validationResult;

            var accessRule = new PositionAccessRule()
            {
                Id = Guid.NewGuid(),
                PositionId = accessRuleDTO.PositionId,
                AttributeDefinitionId = accessRuleDTO.AttributeId,
                Operator = accessRuleDTO.ComparisonOperator,
                Value = accessRuleDTO.Value,
                SecondValue = accessRuleDTO.SecondValue
            };
            await baseRepository.AddItem(accessRule);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }
        public async Task<Result> ModifyAccessRule(
            Guid positionAccessRuleId,
            string value,
            string? secondValue,
            ComparisonOperator comparisonOperator)
        {
            var positionAccessRule = await positionRepository.GetAccessRule(positionAccessRuleId);
            if (positionAccessRule is null)
                return Result.Fail($"Position access rule with id - {positionAccessRuleId} was not found");

            var accessRuleDTO = new AccessRuleDTO(
                positionAccessRule.AttributeDefinitionId,
                positionAccessRule.PositionId,
                comparisonOperator,
                value,
                secondValue);

            var validationResult = await ValidateAccessRule(accessRuleDTO);
            if (validationResult.IsFailed)
                return validationResult;

            positionAccessRule.Value = value;
            positionAccessRule.SecondValue = secondValue;
            positionAccessRule.Operator = comparisonOperator;

            await baseRepository.SaveChanges();
            return Result.Ok();
        }
        public async Task<Result> DeleteAccessRule(Guid Id)
        {
            var positionAccessRule = await positionRepository.GetAccessRule(Id);
            if (positionAccessRule is null)
                return Result.Fail($"Position access rule with id - {Id} was not found");
            await baseRepository.DeleteItems<PositionAccessRule>(new List<Guid> { positionAccessRule.Id });
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

       public async Task<Result<List<PositionAccessRule>>> GetAccessRules(Guid positionId)
        {
            var position = await positionRepository.GetPosition(positionId);
            if (position is null)
                return Result.Fail($"Position with id - {positionId} was not found");
            var accessRules = await positionRepository.GetAccessRules(positionId);
            return accessRules;
        } 

        private async Task<Result> ValidateAccessRule(AccessRuleDTO accessRuleDTO)
        {
            var position = await positionRepository.GetPosition(accessRuleDTO.PositionId);
            if (position is null) return Result.Fail($"Position with id - {accessRuleDTO.PositionId} was not found");
            var attribute = await attributeRepository.GetAttribute(accessRuleDTO.AttributeId.ToString());
            if (attribute is null) return Result.Fail($"Attribute with id - {accessRuleDTO.AttributeId} was not found");
            if (attribute.DataType == DataType.Image) return Result.Fail("You can't apply any access rule with images");
            if (string.IsNullOrEmpty(accessRuleDTO.Value)) return Result.Fail("Value can't be empty"); 
            if (!Allowed.TryGetValue(attribute.DataType, out var operators))
                return Result.Fail("This attribute type cannot be used in access rules");
            if (!operators.Contains(accessRuleDTO.ComparisonOperator)) return Result.Fail("Operator is not valid for this attribute type");
            if ((accessRuleDTO.ComparisonOperator == ComparisonOperator.Between && accessRuleDTO.SecondValue is null) ||
                (accessRuleDTO.ComparisonOperator != ComparisonOperator.Between && accessRuleDTO.SecondValue is not null))
            {
                return Result.Fail("Invalid second value for the comparison operator");
            }
            var value1DataTypeValidationResult = await attributeValueValidator.ValidateValue(accessRuleDTO.Value
                ,attribute.DataType, attribute.Id.ToString());
            if (value1DataTypeValidationResult.IsFailed) return value1DataTypeValidationResult.ToResult();
            if (accessRuleDTO.SecondValue is not null)
            {
                var value2DataTypeValidationResult = await attributeValueValidator.ValidateValue(accessRuleDTO.SecondValue
                    ,attribute.DataType, attribute.Id.ToString());
                if (value2DataTypeValidationResult.IsFailed) return value2DataTypeValidationResult.ToResult();
            }
            return Result.Ok();
        }
        private static readonly Dictionary<DataType, ComparisonOperator[]> Allowed = new()
        {
            [DataType.Numeric] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals,
                ComparisonOperator.Greater,
                ComparisonOperator.Less,
                ComparisonOperator.GreaterOrEqual,
                ComparisonOperator.LessOrEqual,
                ComparisonOperator.Between
           ],
            [DataType.Date] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals,
                ComparisonOperator.Greater,
                ComparisonOperator.Less,
                ComparisonOperator.GreaterOrEqual,
                ComparisonOperator.LessOrEqual,
                ComparisonOperator.Between
           ],
            [DataType.Period] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals,
                ComparisonOperator.Between
           ],
            [DataType.Boolean] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals
           ],
            [DataType.String] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals,
                ComparisonOperator.Contains,
                ComparisonOperator.NotContains,
                ComparisonOperator.StartsWith,
                ComparisonOperator.EndsWith
           ],
            [DataType.Text] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals,
                ComparisonOperator.Contains,
                ComparisonOperator.NotContains
           ],
            [DataType.OneOfMany] = [
               ComparisonOperator.Equals,
                ComparisonOperator.NotEquals
           ]
        };
    }
}

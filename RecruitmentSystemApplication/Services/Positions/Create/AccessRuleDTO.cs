
using RecruitmentSystemDomain.Enums;

namespace RecruitmentSystemApplication.Services.Positions.Create
{
    public record AccessRuleDTO(Guid AttributeId
        ,Guid PositionId
        ,ComparisonOperator ComparisonOperator
        ,string Value
        ,string? SecondValue);
}

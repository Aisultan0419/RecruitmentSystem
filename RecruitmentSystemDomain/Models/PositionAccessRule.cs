using RecruitmentSystemDomain.Enums;
namespace RecruitmentSystemDomain.Models
{
    public class PositionAccessRule
    {
        public Guid Id { get; set; }
        public Guid PositionId { get; set; }
        public Guid AttributeDefinitionId { get; set; }

        public ComparisonOperator Operator { get; set; }
        public required string Value { get; set; }
        public string? SecondValue { get; set; }   
    }
}

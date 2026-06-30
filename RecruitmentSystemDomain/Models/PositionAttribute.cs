using RecruitmentSystemDomain.Enums;
namespace RecruitmentSystemDomain.Models
{
    public class PositionAttribute
    {
        public Guid Id { get; set; }
        public Guid PositionId { get; set; }
        public Guid AttributeDefinitionId { get; set; }
    }
}

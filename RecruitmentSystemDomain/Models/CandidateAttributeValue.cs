namespace RecruitmentSystemDomain.Models
{
    public class CandidateAttributeValue
    {
        public Guid Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid AttributeId { get; set; }
        
        public int Version { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

namespace RecruitmentSystemDomain.Models
{
    public class CandidateAttributeValue
    {
        public Guid Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public Guid UserProfileId { get; set; }
        public Guid AttributeId { get; set; }
        public UserProfile UserProfile { get; set; } = null!;
        public AttributeDefinition AttributeDefinition { get; set; } = null!;

        public int Version { get; set; } = 0;
        public DateTime UpdatedAt { get; set; }
    }
}

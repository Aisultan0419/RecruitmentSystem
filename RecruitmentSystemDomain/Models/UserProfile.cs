
namespace RecruitmentSystemDomain.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }         
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Location { get; set; }
        public int Version { get; set; }           
        public DateTime? UpdatedAt { get; set; }
        public ICollection<CandidateProject> CandidateProjects { get; set; } = [];
        public ICollection<CandidateAttributeValue> CandidateAttributeValues { get; set; } = [];
    }
}

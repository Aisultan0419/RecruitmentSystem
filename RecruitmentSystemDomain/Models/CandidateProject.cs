
namespace RecruitmentSystemDomain.Models
{
    public class CandidateProject
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public List<Tag> Tags { get; set; } = [];
        public required UserProfile UserProfile { get; set; }
    }
}

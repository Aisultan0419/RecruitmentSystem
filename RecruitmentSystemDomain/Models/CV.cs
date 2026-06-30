using RecruitmentSystemDomain.Enums;
namespace RecruitmentSystemDomain.Models
{
    public class CV
    {
        public Guid Id { get; set; }
        public required CVState CVState { get; set; }
        public bool IsAccessRevoked { get; set; }   
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid PositionId { get; set; }
        public Guid UserId { get; set; }
    }
}
    
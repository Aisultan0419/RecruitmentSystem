
namespace RecruitmentSystemDomain.Models
{
    public class Like
    {
        public Guid Id { get; set; }

        public required Guid UserId { get; set; }
        public required Guid CVId { get; set; }
    }
}

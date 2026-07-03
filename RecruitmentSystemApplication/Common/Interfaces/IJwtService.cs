using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindUser(string email);
    }
}

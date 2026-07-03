using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemInfrastructure.Repositories
{
    public class UserRepository(AppDbContext _context) : IUserRepository
    {
        public async Task<User?> FindUser(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}

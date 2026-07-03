using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
namespace RecruitmentSystemInfrastructure
{
    public class BaseRepository(AppDbContext _context) : IBaseRepository
    {
        public async Task AddItem<T>(T item) where T : class
        {
            await _context.Set<T>().AddAsync(item);
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}

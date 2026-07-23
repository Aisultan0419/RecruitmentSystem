using Amazon.Runtime.Telemetry;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;
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
        public async Task DeleteItems<T>(List<Guid> itemIds) where T : class 
        {
            await _context.Set<T>().Where(a => itemIds.Contains(EF.Property<Guid>(a, "Id")))
                .ExecuteDeleteAsync();
        }
    }
}

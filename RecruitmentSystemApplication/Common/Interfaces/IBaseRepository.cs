
namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IBaseRepository
    {
        Task AddItem<T>(T Item) where T : class;
        Task SaveChanges();
        Task DeleteItems<T>(List<Guid> itemIds) where T : class;
    }
}

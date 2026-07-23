namespace RecruitmentSystemApplication.Services.Profile
{
    public interface IImageService
    {
        Task<string> SaveImage(Stream fileStream, bool isAvatar);
    }
}

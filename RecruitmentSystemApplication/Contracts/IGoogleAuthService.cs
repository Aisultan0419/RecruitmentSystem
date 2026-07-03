using FluentResults;
namespace RecruitmentSystemApplication.Contracts
{
    public interface IGoogleAuthService
    {
        Task<string> GoogleAuthAsync(string googleClientToken);
    }
}

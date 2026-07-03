

namespace RecruitmentSystemApplication.Services.Auth.Register
{
    public interface IGoogleValidationService
    {
        Task<GoogleUserPayload?> ValidateGoogleToken(string googleClientToken);
    }
}

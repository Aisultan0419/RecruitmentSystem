namespace RecruitmentSystemApplication.Services.Auth.Register
{
    public record UserRegisterDTO
    (
        string Email,
        string? plainPassword,
        string? FirstName,
        string? LastName,
        string? Location
    );
}
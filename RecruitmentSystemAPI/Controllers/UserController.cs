using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Contracts;
using RecruitmentSystemApplication.Services.Auth.Login;
using RecruitmentSystemApplication.Services.Auth.Register;
using RecruitmentSystemApplication.Services.Profile;
using System.Reflection;
using System.Security.Claims;
namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    (
        IRegistrationService _registerService,
        IGoogleAuthService _googleAuthService,
        ILoginService _loginService,
        IUserProfileService _userProfileService
    ) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO userRegisterDTO)
        {
            var user = await _registerService.RegisterAsync(userRegisterDTO);
            return Ok(user.Id);
        }
        [HttpPost("auth-google")]
        public async Task<ActionResult<string>> RegisterUserViaGoogle([FromQuery] string googleClientToken)
        {
            string userToken = await _googleAuthService.GoogleAuthAsync(googleClientToken);
            return Ok(userToken);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginQueryDTO loginDTO)
        {
            var result = await _loginService.LoginUserAsync(loginDTO);
            return ResultExtensions.ToActionResult(result);
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized("Invalid token");
            }
            var userProfile = await _userProfileService.Get(userId);
            return userProfile;
        }
        [Authorize]
        [HttpPut("profile-modify")]
        public async Task<IActionResult> ModifyUserProfile(UserProfileDTO userProfileDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized("Invalid token");
            }
            var result = await _userProfileService.Modify(userId, userProfileDTO);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized("Invalid token");
            }
            using (Stream stream = file.OpenReadStream())
            {
                if (file.ContentType is null)
                {
                    return BadRequest("File content type is not set");
                }
                var result = await _userProfileService.UploadAvatarAsync(stream, file.ContentType, userId);

                return result.ToActionResult();
            }
        }
    }
}

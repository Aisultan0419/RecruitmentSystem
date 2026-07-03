using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemApplication.Contracts;
using RecruitmentSystemApplication.Services.Auth.Login;
using RecruitmentSystemApplication.Services.Auth.Register;
namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    (
        IRegistrationService _registerService,
        IGoogleAuthService _googleAuthService,
        ILoginService _loginService
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
    }
}

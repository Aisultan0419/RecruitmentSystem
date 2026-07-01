using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemApplication.Contracts;
using RecruitmentSystemApplication.Services.Auth.Register;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    (
        IRegistrationService _register    
    ) : ControllerBase
    {
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO userRegisterDTO)
        {
            var result = await _register.RegisterAsync(userRegisterDTO);
            return Ok(result);
        }
    }
}

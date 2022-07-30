using Alexandria.Api.Data;
using Alexandria.Api.Models.User;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Registers User
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            _logger.LogInformation($"Registration attempt for {userDto.Email}");

            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await _userManager.CreateAsync(user, userDto.Password);

                #region Handle Error - User could not be created
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                #endregion

                #region Add User to specific Role
                await _userManager.AddToRoleAsync(user, userDto.Role);
                #endregion

                return Accepted();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Registration failed in {nameof(Register)}");
                return Problem($"Something went wrong in {nameof(Register)}", statusCode: 500);
            }
        }
        #endregion

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUserDto userDto) { 

            _logger.LogInformation($"Login attempt for {userDto.Email}");

            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);

                if(user == null || !passwordValid)
                {
                    return NotFound();
                }

                return Accepted();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Registration failed in {nameof(Login)}");
                return Problem($"Something went wrong in {nameof(Login)}", statusCode: 500);
            }
        }
    }
}

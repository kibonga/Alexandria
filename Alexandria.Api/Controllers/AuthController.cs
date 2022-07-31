using Alexandria.Api.Data;
using Alexandria.Api.Models.User;
using Alexandria.Api.Static;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Alexandria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        #region Registers User
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            _logger.LogInformation($"Registration attempt for {userDto.Email}");

            try
            {
                #region Map Dto to User
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                #endregion

                #region Create User
                var result = await _userManager.CreateAsync(user, userDto.Password); 
                #endregion

                #region Handle Error - Failed to Create User
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

        #region Login User
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {

            _logger.LogInformation($"Login attempt for {userDto.Email}");

            try
            {
                #region Find User and Check Passwords
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);
                #endregion

                #region Check if Authorized
                if (user == null || !passwordValid)
                {
                    return Unauthorized(userDto);
                }
                #endregion

                #region Create Token
                string token = await GenerateToken(user);
                #endregion

                #region Generate Auth Response
                var authResponse = new AuthResponse
                {
                    Token = token,
                    Email = user.Email,
                    UserId = user.Id,
                };
                #endregion

                return authResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Registration failed in {nameof(Login)}");
                return Problem($"Something went wrong in {nameof(Login)}", statusCode: 500);
            }
        } 
        #endregion

        #region Generates JWT Token 
        private async Task<string> GenerateToken(ApiUser user)
        {
            #region Create Key and Credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            #endregion

            #region Get Roles for User
            var roles = await _userManager.GetRolesAsync(user);
            #endregion

            #region Create List of RoleClaims using Projection
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
            #endregion

            #region Create List of UserClaims using Projection
            var userClaims = await _userManager.GetClaimsAsync(user);
            #endregion

            #region Add Claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);
            #endregion

            #region Generate Jwt Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
            );
            #endregion

            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
        #endregion
    }
}

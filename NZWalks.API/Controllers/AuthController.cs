using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly ILogger<AuthController> logger;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository, ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.logger = logger;
        }

        // POST: api/Auth/Regiester
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegiesterRequestDto regiesterRequestDto)
        {
            // TODO: apply mapping from RegiesterRequestDto to IdentityUser using AutoMapper

            var identityUser = new IdentityUser
            {
                UserName = regiesterRequestDto.Username,
                Email = regiesterRequestDto.Username
            };

            var identittResult = await userManager.CreateAsync(identityUser, regiesterRequestDto.Password);

            if (identittResult.Succeeded)
            {
                // Add roles to this user
                if (regiesterRequestDto.Roles != null && regiesterRequestDto.Roles.Any())
                    identittResult = await userManager.AddToRolesAsync(identityUser, regiesterRequestDto.Roles);

                if (identittResult.Succeeded)
                {
                    logger.LogInformation($"User {regiesterRequestDto.Username} regiestered successfully with roles: {string.Join(", ", regiesterRequestDto.Roles)}");
                    return Ok("User Regiestered! Please login.");
                }
            }

            logger.LogError($"User {regiesterRequestDto.Username} regiesteration failed. Errors: {string.Join(", ", identittResult.Errors.Select(e => e.Description))}");
            return BadRequest("User Regiesteration failed! Please try again.");
        }

        // POST: api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var identityUser = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (identityUser == null)
                return BadRequest("Invalid username!");

            var isValidPassword = await userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password);
            if (isValidPassword)
            {
                var roles = await userManager.GetRolesAsync(identityUser);
                if (roles != null)
                {
                    // Generate JWT token and return to client
                    var jwtToken = tokenRepository.CreateTWTToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto
                    {
                        JwtToken = jwtToken,
                        //UserId = identityUser.Id,
                        //Username = identityUser.UserName,
                        //Roles = roles.ToList()
                    };

                    logger.LogInformation($"User {loginRequestDto.Username} logged in successfully with token: {jwtToken}");
                    return Ok(response);
                }
                else
                    return BadRequest("User has no roles assigned!");
            }
            else
                return BadRequest("Invalid password!");
        }
    }
}

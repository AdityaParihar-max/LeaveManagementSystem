using Microsoft.AspNetCore.Mvc;
using myLeave.Services.Interfaces;
using myLeave.Dtos;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using myLeave.Data;

namespace myLeave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IExceptionLogger _exceptionLogger;

        public AuthController(
            IAuthService authService,
            IConfiguration configuration,
            AppDbContext context,
            IExceptionLogger exceptionLogger)
        {
            _authService = authService;
            _configuration = configuration;
            _context = context;
            _exceptionLogger = exceptionLogger;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            try
            {
                var user = _context.Employees
                    .FirstOrDefault(e => e.Email == loginDto.Email && e.Password == loginDto.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
                };

                var keyString = _configuration["JwtSettings:Key"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];

                if (string.IsNullOrEmpty(keyString))
                    throw new InvalidOperationException("JWT key is not configured.");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    role = user.Role ?? "",
                    email = user.Email ?? ""
                });
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogAsync(ex);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] EmployeeRegisterDto dto)
        {
            try
            {
                var success = await _authService.RegisterAsync(dto);
                if (!success)
                    return Conflict(new { message = "Email already registered" });

                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                await _exceptionLogger.LogAsync(ex);
                return BadRequest(new { message = "An error occurred during registration" });
            }
        }
    }
}

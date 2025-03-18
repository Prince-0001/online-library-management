using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiTemplate.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.DTOs;

namespace WebApiTemplate.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new IdentityUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Check if the 'User' role exists, and create it if it doesn't
                var userRoleExists = await _roleManager.RoleExistsAsync("User");
                if (!userRoleExists)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                    if (!roleResult.Succeeded)
                    {
                        return new BadRequestObjectResult(roleResult.Errors);
                    }
                }

                // Assign the 'User' role to the newly created user
                await _userManager.AddToRoleAsync(user, "User");

                return new OkObjectResult(new { message = "User registered successfully with 'User' role" });
            }

            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", user.Id),
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256)
                );
                return new OkObjectResult(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return new UnauthorizedResult();
        }

        public async Task<IActionResult> AddRole(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    return new OkObjectResult(new { message = "Role Added Successfully" });
                }
                return new BadRequestObjectResult(result.Errors);
            }
            return new BadRequestObjectResult("Role already Exists");
        }

        public async Task<IActionResult> AssignRole(UserRoleDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return new BadRequestObjectResult("User not found");
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { message = "Role assigned successfully" });
            }
            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<IActionResult> GetRole(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new OkObjectResult(new { Username = username, Roles = roles });
        }
    }
}

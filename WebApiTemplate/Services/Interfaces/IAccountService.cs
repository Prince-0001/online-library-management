using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.DTOs;

namespace WebApiTemplate.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IActionResult> Register(RegisterDto model);
        Task<IActionResult> Login(LoginDto model);
        Task<IActionResult> AddRole(string role);
        Task<IActionResult> AssignRole(UserRoleDto model);

        Task<IActionResult> GetRole(string username);
    }
}

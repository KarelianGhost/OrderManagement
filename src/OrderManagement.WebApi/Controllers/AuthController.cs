using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.Auth;
using OrderManagement.Application.Interfaces;

namespace OrderManagement.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(response);
    }
}
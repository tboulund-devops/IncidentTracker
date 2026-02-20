using Application.Common.Interfaces.Features;
using Application.Common.Results;
using Application.DTOs.Responses;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Domain.Enums;
using Domain.Settings;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthFeature authFeature, AppSettings appSettings) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand loginRequest)
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            return NoContent();
        }
        
        var loginResponseDto = await authFeature.HandleLogin(loginRequest);
        
        var cookieOptionsAccess = CookieHelper.CreateAccessTokenCookieOptions(appSettings.JwtSettings.AccessTokenLifetime);
        var cookieOptionsRefresh = CookieHelper.CreateRefreshTokenCookieOptions(appSettings.JwtSettings.RefreshTokenLifetime);

        Response.Cookies.Append("accessToken", loginResponseDto.Dto!.AccessToken, cookieOptionsAccess);
        Response.Cookies.Append("refreshToken", loginResponseDto.Dto!.RefreshToken, cookieOptionsRefresh);
        
        return loginResponseDto.Status switch
        {
            ResultStatus.Unauthorized => Unauthorized(),
            ResultStatus.Failure => BadRequest(),
            ResultStatus.Success => Ok(loginResponseDto.Dto.User),
            // ResultStatus.Forbidden => BadRequest(loginResponseDto),
            // ResultStatus.NotFound => BadRequest(loginResponseDto),
            // _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerRequest)
    {
        if (User.Identity is { IsAuthenticated: false } || User.Identity is { IsAuthenticated: true } && User.IsInRole(nameof(RoleType.User)))
        {
            if (registerRequest.Role is RoleType.Admin or RoleType.Crew)
            {
                return Unauthorized("You are not authorized to register this user.");
            } 
            var registerResult = await authFeature.HandleRegisterUser(registerRequest);
            if (registerResult.IsSuccess)
            {
                return Ok(registerResult);
            }
        }

        if (User.IsInRole(nameof(RoleType.Crew)))
        {
            if (registerRequest.Role is RoleType.Admin or RoleType.User)
            {
                return Unauthorized("You are not authorized to register this user.");
            }
            
            var registerResult = await authFeature.HandleRegisterUser(registerRequest);
            if (registerResult.IsSuccess)
            {
                return Ok(registerResult);
            }
        }

        if (User.IsInRole(nameof(RoleType.Admin)))
        {
            var registerResult = await authFeature.HandleRegisterUser(registerRequest);
            if (registerResult.IsSuccess)
            {
                return Ok(registerResult);
            }
        }
        
        return BadRequest();
    }
}
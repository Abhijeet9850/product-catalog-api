using Asp.Versioning;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public record LoginRequest(string Username, string Password);

    public record RefreshRequest(string AccessToken, string RefreshToken);

    public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public ActionResult<TokenResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Unauthorized();

            var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(request.Username, "Admin");
            var refreshToken = _tokenService.GenerateRefreshToken();

            return Ok(new TokenResponse(accessToken, refreshToken, expiresAt));
        }

        [HttpPost("refresh")]
        public ActionResult<TokenResponse> Refresh([FromBody] RefreshRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal?.Identity?.Name is null)
                return Unauthorized();

            var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(principal.Identity.Name, "Admin");
            var refreshToken = _tokenService.GenerateRefreshToken();

            return Ok(new TokenResponse(accessToken, refreshToken, expiresAt));
        }
    }
}
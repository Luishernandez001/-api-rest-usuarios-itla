using ApiUsuarios.Data;
using ApiUsuarios.Models;
using ApiUsuarios.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiUsuarios.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, TokenService tokenService, IConfiguration config)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var hashedPassword = TokenService.HashPassword(request.Password);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario
                                       && u.Password == hashedPassword);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });

            var accessToken = _tokenService.GenerateAccessToken(usuario);
            var refreshToken = _tokenService.GenerateRefreshToken(usuario);
            var expiration = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpirationMinutes"]!));

            return Ok(new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = expiration
            });
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public ActionResult<LoginResponse> Refresh([FromBody] RefreshRequest request)
        {
            var principal = _tokenService.GetPrincipalFromRefreshToken(request.RefreshToken);

            if (principal == null)
                return Unauthorized(new { mensaje = "El refresh token es inválido o ha expirado." });

            var userId = int.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var nombreUsuario = principal.FindFirstValue(JwtRegisteredClaimNames.UniqueName)!;

            var usuario = new Usuario { Id = userId, NombreUsuario = nombreUsuario };

            var newAccessToken = _tokenService.GenerateAccessToken(usuario);
            var newRefreshToken = _tokenService.GenerateRefreshToken(usuario);
            var expiration = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpirationMinutes"]!));

            return Ok(new LoginResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Expiration = expiration
            });
        }
    }
}

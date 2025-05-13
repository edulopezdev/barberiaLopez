using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "El email y la contraseña son obligatorios.",
                    }
                );
            }

            var usuario = await _context.Usuario.SingleOrDefaultAsync(u =>
                u.Email == request.Email
            );

            if (
                usuario == null
                || string.IsNullOrEmpty(usuario.PasswordHash)
                || !VerifyPassword(request.Password, usuario.PasswordHash)
            )
            {
                return Unauthorized(
                    new
                    {
                        status = 401,
                        error = "Unauthorized",
                        message = "Email o contraseña incorrectos.",
                    }
                );
            }

            var token = GenerateJwtToken(usuario);

            return Ok(
                new
                {
                    status = 200,
                    message = "Inicio de sesión exitoso.",
                    token,
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.RolId,
                    },
                }
            );
        }

        // Método auxiliar para verificar contraseña
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        // Método auxiliar para generar token JWT
        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(
                _configuration["Jwt:Key"]
                    ?? throw new ArgumentNullException(
                        "Jwt:Key no está definido en appsettings.json"
                    )
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email ?? ""),
                        new Claim(ClaimTypes.Role, usuario.RolId.ToString()),
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

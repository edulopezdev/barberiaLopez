using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // protección general de todos los endpoints
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios (Lista de usuarios)
        [HttpGet]
        public IActionResult GetUsuarios(int page = 1, int pageSize = 10)
        {
            var totalUsuarios = _context.Usuario.Count(u => u.Activo);
            var usuarios = _context
                .Usuario.Where(u => u.Activo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = totalUsuarios > 0
                        ? "Lista de usuarios obtenida correctamente."
                        : "No hay usuarios disponibles.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize),
                        currentPage = page,
                        pageSize,
                        totalUsuarios,
                    },
                    usuarios = usuarios ?? new List<Usuario>(), // esto es para evitar nulls
                }
            );
        }

        // GET: api/usuarios/{id} (Un usuario específico)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        error = "Not Found",
                        message = "El usuario no existe.",
                    }
                );
            }

            return Ok(
                new
                {
                    status = 200,
                    message = "Usuario encontrado.",
                    usuario,
                }
            );
        }

        // POST: api/usuarios (Crear un nuevo usuario)
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult PostUsuario([FromBody] Usuario usuario)
        {
            var usuarioIdLogueado = 0; // ⚡ Temporalmente asignamos "0" hasta que haya autenticación

            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new { status = 400, message = "Error en la validación de los datos." }
                );
            }

            usuario.IdUsuarioCrea = usuarioIdLogueado;
            usuario.FechaRegistro = DateTime.UtcNow;

            if (usuario.AccedeAlSistema)
            {
                if (string.IsNullOrEmpty(usuario.Password))
                {
                    return BadRequest(
                        new { status = 400, message = "El usuario requiere una contraseña." }
                    );
                }
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }

            _context.Usuario.Add(usuario);
            _context.SaveChanges();

            // ✅ Ahora devolvemos un JSON con los datos del usuario creado
            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.Id },
                new
                {
                    status = 201,
                    message = "Usuario creado exitosamente.",
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Telefono,
                        usuario.Avatar,
                        usuario.RolId,
                        usuario.AccedeAlSistema,
                        usuario.Activo,
                        usuario.FechaRegistro,
                        usuario.IdUsuarioCrea,
                    },
                }
            );
        }

        // PUT: api/usuarios/{id} (Actualizar usuario)
        [HttpPut("{id}")]
        public IActionResult PutUsuario(int id, Usuario usuario)
        {
            var usuarioIdLogueado = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
            );
            var esAdministrador = User.IsInRole("Administrador");

            if (usuarioIdLogueado != id && !esAdministrador)
            {
                return Unauthorized(
                    new
                    {
                        status = 401,
                        message = "No tienes permisos para modificar este usuario.",
                    }
                );
            }

            var usuarioExistente = _context.Usuario.Find(id);
            if (usuarioExistente == null)
            {
                return NotFound(new { status = 404, message = "El usuario no existe." });
            }

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Email = usuario.Email;
            usuarioExistente.Telefono = usuario.Telefono;
            usuarioExistente.Avatar = usuario.Avatar;
            usuarioExistente.RolId = usuario.RolId;
            usuarioExistente.AccedeAlSistema = usuario.AccedeAlSistema;
            usuarioExistente.Activo = usuario.Activo;
            usuarioExistente.IdUsuarioModifica = usuarioIdLogueado; // Guardamos quién modificó
            usuarioExistente.FechaModificacion = DateTime.UtcNow; //  Guardamos fecha de modificación

            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                usuarioExistente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    usuario.PasswordHash
                );
            }

            _context.Entry(usuarioExistente).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(
                new
                {
                    status = 200,
                    message = "Usuario actualizado correctamente.",
                    usuario = usuarioExistente,
                }
            );
        }

        // DELETE: api/usuarios/{id} (Eliminación lógica)
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Eliminación lógica: marcar el usuario como inactivo
            usuario.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

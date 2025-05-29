using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Dtos;
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

        // GET: api/usuarios (lista de usuarios con roles distintos a 3)
        [HttpGet]
        public IActionResult GetUsuarios(
            int page = 1,
            int pageSize = 10,
            string? nombre = null,
            string? email = null,
            string? telefono = null,
            string? rolNombre = null,
            bool? activo = null
        )
        {
            var query = _context.Usuario.Include(u => u.Rol).Where(u => u.RolId != 3).AsQueryable();

            // Ordenamiento
            string? ordenarPor = HttpContext.Request.Query["ordenarPor"];
            string? ordenarDesc = HttpContext.Request.Query["ordenDescendente"];
            bool ordenarDescendente = ordenarDesc == "true";

            if (!string.IsNullOrEmpty(ordenarPor))
            {
                ordenarPor = ordenarPor.ToLower();
                query = ordenarPor switch
                {
                    "nombre" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Nombre)
                        : query.OrderBy(u => u.Nombre),
                    "email" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email),
                    "telefono" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Telefono)
                        : query.OrderBy(u => u.Telefono),
                    "activo" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Activo)
                        : query.OrderBy(u => u.Activo),
                    "rolnombre" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Rol!.NombreRol)
                        : query.OrderBy(u => u.Rol!.NombreRol),
                    _ => query,
                };
            }
            else
            {
                query = query.OrderBy(u => u.Nombre);
            }

            // Filtros
            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(u => u.Nombre!.Contains(nombre));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email!.Contains(email));
            if (!string.IsNullOrEmpty(telefono))
                query = query.Where(u => u.Telefono!.Contains(telefono));
            if (!string.IsNullOrEmpty(rolNombre))
                query = query.Where(u => u.Rol!.NombreRol.Contains(rolNombre));
            if (activo.HasValue)
                query = query.Where(u => u.Activo == activo.Value);

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre!,
                    Email = u.Email!,
                    Telefono = u.Telefono!,
                    Activo = u.Activo,
                    RolId = u.RolId,
                    RolNombre = u.Rol!.NombreRol,
                })
                .ToList();

            string mensaje =
                total > 0
                    ? "Usuarios obtenidos correctamente."
                    : "No se encontraron usuarios con esos filtros.";

            return Ok(
                new
                {
                    status = 200,
                    message = mensaje,
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)total / pageSize),
                        currentPage = page,
                        pageSize,
                        total,
                    },
                    usuarios = data,
                }
            );
        }

        // GET: api/usuarios/usuarios-sistema
        [HttpGet("usuarios-sistema")]
        public IActionResult GetUsuariosSistema(int page = 1, int pageSize = 10)
        {
            var query = _context.Usuario.Where(u => u.Activo && (u.RolId == 1 || u.RolId == 2));

            var total = query.Count();
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = "Usuarios obtenidos correctamente.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)total / pageSize),
                        currentPage = page,
                        pageSize,
                        total,
                    },
                    usuarios = data,
                }
            );
        }

        // GET: api/usuarios/clientes (lista de clientes)
        [HttpGet("clientes")]
        public IActionResult GetClientes(
            int page = 1,
            int pageSize = 10,
            string? nombre = null,
            string? email = null,
            string? telefono = null,
            bool? activo = null
        )
        {
            // Base query: solo usuarios con RolId = 3 (clientes)
            var query = _context.Usuario.AsQueryable();

            query = query.Where(u => u.RolId == 3);

            // Obtener ordenamiento desde query string manualmente
            string? ordenarPor = HttpContext.Request.Query["ordenarPor"];
            string? ordenarDesc = HttpContext.Request.Query["ordenDescendente"];
            bool ordenarDescendente = ordenarDesc == "true";

            if (!string.IsNullOrEmpty(ordenarPor))
            {
                ordenarPor = ordenarPor.ToLower(); // normalizar

                query = ordenarPor switch
                {
                    "nombre" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Nombre)
                        : query.OrderBy(u => u.Nombre),
                    "email" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email),
                    "telefono" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Telefono)
                        : query.OrderBy(u => u.Telefono),
                    "activo" => ordenarDescendente
                        ? query.OrderByDescending(u => u.Activo)
                        : query.OrderBy(u => u.Activo),
                    _ => query, // si el campo no es válido, no se ordena
                };
            }

            // Filtros dinámicos
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(u => u.Nombre!.Contains(nombre));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email!.Contains(email));
            }
            if (!string.IsNullOrEmpty(telefono))
            {
                query = query.Where(u => u.Telefono!.Contains(telefono));
            }
            if (activo.HasValue)
            {
                query = query.Where(u => u.Activo == activo.Value);
            }

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ClienteDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre!,
                    Email = u.Email!,
                    Telefono = u.Telefono!,
                    Activo = u.Activo,
                })
                .ToList();

            string mensaje =
                total > 0
                    ? "Clientes obtenidos correctamente."
                    : "No se encontraron clientes con esos filtros.";

            return Ok(
                new
                {
                    status = 200,
                    message = mensaje,
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)total / pageSize),
                        currentPage = page,
                        pageSize,
                        total,
                    },
                    clientes = data,
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
        [HttpPost]
        public IActionResult PostUsuario([FromBody] CrearUsuarioDto dto)
        {
            try
            {
                var usuarioIdLogueado = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
                );
                var esAdministrador = User.IsInRole("Administrador");

                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new
                        {
                            status = 400,
                            message = "Datos inválidos.",
                            errors = ModelState,
                        }
                    );
                }

                if (dto.RolId != 3 && !esAdministrador)
                {
                    return Unauthorized(
                        new { status = 401, message = "No puedes crear usuarios con este rol." }
                    );
                }

                // Validación de email duplicado
                var emailNormalizado = dto.Email?.Trim().ToLower();
                bool emailExistente = _context.Usuario.Any(u =>
                    u.Email != null && u.Email.ToLower() == emailNormalizado
                );

                if (emailExistente)
                {
                    return BadRequest(
                        new { status = 400, message = "El email ya está en uso por otro usuario." }
                    );
                }

                var usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    Avatar = dto.Avatar,
                    RolId = dto.RolId,
                    AccedeAlSistema = dto.AccedeAlSistema,
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow,
                    IdUsuarioCrea = usuarioIdLogueado,
                };

                if (usuario.RolId == 3)
                {
                    usuario.AccedeAlSistema = false; // por las dudas
                }

                if (usuario.AccedeAlSistema)
                {
                    if (string.IsNullOrEmpty(dto.Password))
                    {
                        return BadRequest(
                            new
                            {
                                status = 400,
                                message = "La contraseña es obligatoria si el usuario accede al sistema.",
                            }
                        );
                    }

                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                if (!usuario.AccedeAlSistema)
                {
                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123");
                }

                _context.Usuario.Add(usuario);
                _context.SaveChanges();

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
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new { status = 500, message = "Error interno: " + ex.Message }
                );
            }
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public IActionResult PutUsuario(int id, [FromBody] EditarUsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        code = "DATOS_INVALIDOS",
                        message = "Datos inválidos",
                        errors = ModelState,
                    }
                );
            }

            // Intentar obtener el Id del usuario logueado (puede ser null)
            var usuarioIdLogueadoString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int usuarioIdLogueado = 0;
            if (
                !string.IsNullOrEmpty(usuarioIdLogueadoString)
                && !int.TryParse(usuarioIdLogueadoString, out usuarioIdLogueado)
            )
            {
                usuarioIdLogueado = 0; // Por defecto
            }

            var esAdministrador = User.IsInRole("Administrador");

            if (usuarioIdLogueado != id && !esAdministrador)
            {
                return Unauthorized(
                    new
                    {
                        status = 401,
                        code = "SIN_AUTORIZACION",
                        message = "No tienes permisos para modificar este usuario.",
                    }
                );
            }

            var usuarioExistente = _context.Usuario.Find(id);
            if (usuarioExistente == null)
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        code = "USUARIO_NO_ENCONTRADO",
                        message = "El usuario no existe.",
                    }
                );
            }

            var nuevoEmail = dto.Email?.Trim().ToLower();
            var emailActual = usuarioExistente.Email?.Trim().ToLower();

            if (nuevoEmail != emailActual)
            {
                bool emailDuplicado = _context.Usuario.Any(u =>
                    u.Email != null && u.Email.ToLower() == nuevoEmail && u.Id != id
                );

                if (emailDuplicado)
                {
                    return BadRequest(
                        new
                        {
                            status = 400,
                            code = "EMAIL_DUPLICADO",
                            message = "El email ya está en uso por otro usuario.",
                        }
                    );
                }
            }

            // Actualizar campos
            usuarioExistente.Nombre = dto.Nombre;
            usuarioExistente.Email = dto.Email?.Trim() ?? usuarioExistente.Email;
            usuarioExistente.Telefono = dto.Telefono;
            usuarioExistente.Avatar = dto.Avatar;
            usuarioExistente.RolId = dto.RolId;
            usuarioExistente.AccedeAlSistema = dto.AccedeAlSistema;
            usuarioExistente.Activo = dto.Activo ?? usuarioExistente.Activo;
            usuarioExistente.IdUsuarioModifica = usuarioIdLogueado;
            usuarioExistente.FechaModificacion = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuarioExistente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _context.SaveChanges();

            return Ok(
                new
                {
                    status = 200,
                    message = "Usuario actualizado correctamente.",
                    usuario = new
                    {
                        usuarioExistente.Id,
                        usuarioExistente.Nombre,
                        usuarioExistente.Email,
                        usuarioExistente.Telefono,
                        usuarioExistente.Avatar,
                        usuarioExistente.RolId,
                        usuarioExistente.AccedeAlSistema,
                        usuarioExistente.Activo,
                        usuarioExistente.FechaRegistro,
                        usuarioExistente.FechaModificacion,
                    },
                }
            );
        }

        // DELETE: api/usuarios/{id} (Eliminación lógica)
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

        // PATCH: api/usuarios/{id}/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoUsuario(
            int id,
            [FromBody] CambiarEstadoDto dto
        )
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { status = 404, message = "El usuario no existe." });
            }

            usuario.Activo = dto.Activo;
            usuario.FechaModificacion = DateTime.UtcNow;
            usuario.IdUsuarioModifica = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
            );

            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    status = 200,
                    message = dto.Activo
                        ? "Usuario restaurado correctamente."
                        : "Usuario desactivado correctamente.",
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Activo,
                    },
                }
            );
        }
    }
}

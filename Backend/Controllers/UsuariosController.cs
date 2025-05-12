using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpPost]
        public IActionResult PostUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(e => e.Value.Errors.Any())
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                    );

                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "Error en la validación de los datos.",
                        details = errores,
                    }
                );
            }

            _context.Usuario.Add(usuario);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.Id },
                new
                {
                    status = 201,
                    message = "Usuario creado correctamente.",
                    usuario,
                }
            );
        }

        // PUT: api/usuarios/{id} (Actualizar usuario)
        [HttpPut("{id}")]
        public IActionResult PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "El ID en la URL no coincide con el ID del cuerpo de la solicitud.",
                    }
                );
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuario.Any(e => e.Id == id))
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
                throw;
            }

            return Ok(
                new
                {
                    status = 200,
                    message = "Usuario actualizado correctamente.",
                    usuario,
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
    }
}

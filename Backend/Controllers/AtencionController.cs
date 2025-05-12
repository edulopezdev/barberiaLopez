using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtencionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AtencionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/atencion (Lista de atenciones)
        [HttpGet]
        public IActionResult GetAtenciones(int page = 1, int pageSize = 10)
        {
            var totalAtenciones = _context.Atencion.Count();
            var atenciones = _context
                .Atencion.OrderBy(a => a.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = totalAtenciones > 0
                        ? "Lista de atenciones obtenida correctamente."
                        : "No hay atenciones disponibles.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)totalAtenciones / pageSize),
                        currentPage = page,
                        pageSize,
                        totalAtenciones,
                    },
                    atenciones = atenciones ?? new List<Atencion>(), // esto es para evitar nulls
                }
            );
        }

        // GET: api/atencion/{id} (Una atención específica)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAtencion(int id)
        {
            var atencion = await _context.Atencion.FindAsync(id);
            if (atencion == null)
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        error = "Not Found",
                        message = "La atención no existe.",
                    }
                );
            }

            return Ok(
                new
                {
                    status = 200,
                    message = "Atención encontrada.",
                    atencion,
                }
            );
        }

        // POST: api/atencion (Registrar una nueva atención)
        [HttpPost]
        public async Task<IActionResult> PostAtencion(Atencion atencion)
        {
            if (atencion.Fecha == default)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "La fecha de la atención es obligatoria.",
                    }
                );
            }

            _context.Atencion.Add(atencion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAtencion),
                new { id = atencion.Id },
                new
                {
                    status = 201,
                    message = "Atención registrada correctamente.",
                    atencion,
                }
            );
        }

        // PUT: api/atencion/{id} (Actualizar datos de una atención)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAtencion(int id, Atencion atencion)
        {
            if (id != atencion.Id)
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

            _context.Entry(atencion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Atencion.Any(e => e.Id == id))
                {
                    return NotFound(
                        new
                        {
                            status = 404,
                            error = "Not Found",
                            message = "La atención no existe.",
                        }
                    );
                }
                throw;
            }

            return Ok(
                new
                {
                    status = 200,
                    message = "Atención actualizada correctamente.",
                    atencion,
                }
            );
        }

        // DELETE: api/atencion/{id} (Eliminar una atención)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAtencion(int id)
        {
            var atencion = await _context.Atencion.FindAsync(id);
            if (atencion == null)
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        error = "Not Found",
                        message = "La atención no existe.",
                    }
                );
            }

            // Verificar si la atención está vinculada en `detalle_atencion`
            var tieneDependencias = _context.DetalleAtencion.Any(d => d.AtencionId == id);
            if (tieneDependencias)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "No se puede eliminar la atención porque está vinculada a registros en detalle de atención.",
                        details = new
                        {
                            AtencionId = id,
                            Relacion = "DetalleAtencion",
                            Motivo = "Restricción de clave foránea",
                        },
                    }
                );
            }

            _context.Atencion.Remove(atencion);
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    status = 200,
                    message = "Atención eliminada correctamente.",
                    atencionIdEliminado = id,
                }
            );
        }
    }
}

using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosServiciosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosServiciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/productosservicios (Obtener todos los productos)
        [HttpGet("almacenables")]
        public IActionResult GetProductosAlmacenables(int page = 1, int pageSize = 10)
        {
            var query = _context
                .ProductosServicios.Where(p => p.EsAlmacenable ?? false) // aca manejamos correctamente el null
                .OrderBy(p => p.Nombre);

            var totalProductos = query.Count();
            var productos = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = totalProductos > 0
                        ? "Lista de productos almacenables obtenida correctamente."
                        : "No hay productos almacenables disponibles.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)totalProductos / pageSize),
                        currentPage = page,
                        pageSize,
                        totalProductos,
                    },
                    productos = productos ?? new List<ProductoServicio>(), // esto es para evitar nulls
                }
            );
        }

        // GET: api/productosservicios (Obtener todos los servicios)
        [HttpGet("noAlmacenables")]
        public IActionResult GetProductosNoAlmacenables(int page = 1, int pageSize = 10)
        {
            var query = _context
                .ProductosServicios.Where(p => !(p.EsAlmacenable ?? false)) // aca con el ! manejamos correctamente el null
                .OrderBy(p => p.Nombre);

            var totalProductos = query.Count();
            var productos = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = totalProductos > 0
                        ? "Lista de productos no almacenables obtenida correctamente."
                        : "No hay productos no almacenables disponibles.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)totalProductos / pageSize),
                        currentPage = page,
                        pageSize,
                        totalProductos,
                    },
                    productos = productos ?? new List<ProductoServicio>(), // esto es para evitar nulls
                }
            );
        }

        // GET: api/productosservicios/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoServicio(int id)
        {
            var productoServicio = await _context.ProductosServicios.FindAsync(id);
            if (productoServicio == null)
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        error = "Not Found",
                        message = "El producto o servicio no existe.",
                    }
                );
            }

            return Ok(
                new
                {
                    status = 200,
                    message = "Producto o servicio encontrado.",
                    productoServicio,
                }
            );
        }

        // POST: api/productosservicios
        [HttpPost]
        public async Task<IActionResult> PostProductoServicio(ProductoServicio productoServicio)
        {
            if (string.IsNullOrEmpty(productoServicio.Nombre))
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "El nombre del producto o servicio es obligatorio.",
                    }
                );
            }

            // Validación: Un producto no almacenable no puede tener cantidad mayor a 0
            if (!(productoServicio.EsAlmacenable ?? false) && productoServicio.Cantidad > 0)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "Un producto no almacenable no puede tener cantidad mayor a 0.",
                    }
                );
            }

            _context.ProductosServicios.Add(productoServicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductoServicio),
                new { id = productoServicio.Id },
                new
                {
                    status = 201,
                    message = "Producto o servicio creado correctamente.",
                    productoServicio,
                }
            );
        }

        // PUT: api/productosservicios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductoServicio(
            int id,
            ProductoServicio productoServicio
        )
        {
            if (id != productoServicio.Id)
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

            // 🔹 Validación: Un producto no almacenable no puede tener cantidad mayor a 0
            if (!(productoServicio.EsAlmacenable ?? false) && productoServicio.Cantidad > 0)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "Un producto no almacenable no puede tener cantidad mayor a 0.",
                    }
                );
            }

            _context.Entry(productoServicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProductosServicios.Any(e => e.Id == id))
                {
                    return NotFound(
                        new
                        {
                            status = 404,
                            error = "Not Found",
                            message = "El producto o servicio no existe.",
                        }
                    );
                }
                throw;
            }

            return Ok(
                new
                {
                    status = 200,
                    message = "Producto o servicio actualizado correctamente.",
                    productoServicio,
                }
            );
        }

        // DELETE: api/productosservicios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductoServicio(int id)
        {
            var productoServicio = await _context.ProductosServicios.FindAsync(id);
            if (productoServicio == null)
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        error = "Not Found",
                        message = "El producto o servicio no existe.",
                    }
                );
            }

            // 🔹 Verificar si el producto está vinculado en `detalle_atencion`
            var tieneDependencias = _context.DetalleAtencion.Any(d => d.ProductoServicioId == id);
            if (tieneDependencias)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        error = "Bad Request",
                        message = "No se puede eliminar el producto porque está vinculado a una atención.",
                        details = new
                        {
                            ProductoServicioId = id,
                            Relacion = "DetalleAtencion",
                            Motivo = "Restricción de clave foránea",
                        },
                    }
                );
            }

            _context.ProductosServicios.Remove(productoServicio);
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    status = 200,
                    message = "Producto o servicio eliminado correctamente.",
                    productoEliminado = productoServicio.Nombre,
                }
            );
        }
    }
}

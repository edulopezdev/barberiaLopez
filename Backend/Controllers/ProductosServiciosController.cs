using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Administrador,Barbero")]
        public IActionResult GetProductosAlmacenables(
            int page = 1,
            int pageSize = 10,
            string? sort = null,
            string? order = null,
            string? nombre = null,
            string? descripcion = null,
            decimal? precio = null,
            int? cantidad = null
        )
        {
            var query = _context.ProductosServicios.Where(p => p.EsAlmacenable == true);

            // Filtros seguros
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p =>
                    !string.IsNullOrEmpty(p.Nombre) && p.Nombre.Contains(nombre)
                );
            }

            if (!string.IsNullOrEmpty(descripcion))
            {
                query = query.Where(p =>
                    !string.IsNullOrEmpty(p.Descripcion) && p.Descripcion.Contains(descripcion)
                );
            }

            if (precio.HasValue && precio.Value > 0)
            {
                query = query.Where(p => p.Precio == precio.Value);
            }

            if (cantidad.HasValue && cantidad.Value >= 0)
            {
                query = query.Where(p => p.Cantidad == cantidad.Value);
            }

            // Ordenamiento seguro
            switch (sort?.ToLower())
            {
                case "nombre":
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Nombre)
                            : query.OrderBy(p => p.Nombre);
                    break;

                case "precio":
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Precio)
                            : query.OrderBy(p => p.Precio);
                    break;

                case "cantidad":
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Cantidad)
                            : query.OrderBy(p => p.Cantidad);
                    break;

                default:
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Nombre)
                            : query.OrderBy(p => p.Nombre);
                    break;
            }

            var totalProductos = query.Count();
            var productosDb = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var productosDto = productosDb
                .Select(p =>
                {
                    var imagen = _context.Imagen.FirstOrDefault(i =>
                        i.TipoImagen == "ProductoServicio"
                        && i.IdRelacionado == p.Id
                        && i.Activo == true
                    );

                    return new ProductoServicioConImagenDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        EsAlmacenable = p.EsAlmacenable,
                        Cantidad = p.Cantidad,
                        RutaImagen = imagen?.Ruta,
                    };
                })
                .ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = totalProductos > 0
                        ? "Lista obtenida correctamente."
                        : "No hay productos.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)totalProductos / pageSize),
                        currentPage = page,
                        pageSize,
                        totalProductos,
                    },
                    productos = productosDto,
                }
            );
        }

        // GET: api/productosservicios/noAlmacenables (Obtener todos los servicios)
        [HttpGet("noAlmacenables")]
        [Authorize(Roles = "Administrador,Barbero")]
        public IActionResult GetProductosNoAlmacenables(
            int page = 1,
            int pageSize = 10,
            string? sort = null,
            string? order = null
        )
        {
            var query = _context.ProductosServicios.Where(p => !(p.EsAlmacenable ?? false));

            // Aplicar ordenamiento
            switch (sort?.ToLower())
            {
                case "nombre":
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Nombre)
                            : query.OrderBy(p => p.Nombre);
                    break;

                case "precio":
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Precio)
                            : query.OrderBy(p => p.Precio);
                    break;

                case "descripcion":
                    query =
                        order == "desc"
                            ? query.OrderByDescending(p => p.Descripcion)
                            : query.OrderBy(p => p.Descripcion);
                    break;

                default:
                    // Orden por defecto
                    query = query.OrderBy(p => p.Nombre);
                    break;
            }

            var totalProductos = query.Count();
            var productosDb = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var productosDto = productosDb
                .Select(p =>
                {
                    var imagen = _context.Imagen.FirstOrDefault(i =>
                        i.TipoImagen == "ProductoServicio"
                        && i.IdRelacionado == p.Id
                        && i.Activo == true
                    );

                    return new ProductoServicioConImagenDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        EsAlmacenable = p.EsAlmacenable,
                        Cantidad = p.Cantidad,
                        RutaImagen = imagen?.Ruta,
                    };
                })
                .ToList();

            return Ok(
                new
                {
                    status = 200,
                    message = totalProductos > 0
                        ? "Lista de servicios obtenida correctamente."
                        : "No hay servicios disponibles.",
                    pagination = new
                    {
                        totalPages = (int)Math.Ceiling((double)totalProductos / pageSize),
                        currentPage = page,
                        pageSize,
                        totalProductos,
                    },
                    productos = productosDto,
                }
            );
        }

        // GET: api/productosservicios/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Barbero")]
        public async Task<IActionResult> GetProductoServicio(int id)
        {
            var productoServicio = await _context.ProductosServicios.FindAsync(id);
            if (productoServicio == null)
            {
                return NotFound(
                    new { status = 404, message = "El producto o servicio no existe." }
                );
            }

            // Buscamos imagen relacionada
            var imagen = _context.Imagen.FirstOrDefault(i =>
                i.TipoImagen == "ProductoServicio" && i.IdRelacionado == id && i.Activo == true
            );

            return Ok(
                new
                {
                    status = 200,
                    message = "Producto o servicio encontrado.",
                    productoServicio,
                    imagen,
                }
            );
        }

        // POST: api/productosservicios
        [HttpPost]
        [Authorize(Roles = "Administrador,Barbero")]
        public async Task<IActionResult> PostProductoServicio(
            [FromForm] ProductoServicioCrearDto dto
        )
        {
            if (string.IsNullOrEmpty(dto.Nombre))
                return BadRequest(new { status = 400, message = "El nombre es obligatorio." });

            if (!(dto.EsAlmacenable ?? false) && dto.Cantidad > 0)
                return BadRequest(
                    new { status = 400, message = "Un servicio no puede tener cantidad > 0." }
                );

            var producto = new ProductoServicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                EsAlmacenable = dto.EsAlmacenable,
                Cantidad = dto.Cantidad,
            };

            _context.ProductosServicios.Add(producto);
            await _context.SaveChangesAsync();

            if (dto.Imagen != null && dto.Imagen.Length > 0)
            {
                // Carpeta base según tipo (producto o servicio)
                string baseFolder = (producto.EsAlmacenable ?? false) ? "productos" : "servicios";

                // Ruta completa: wwwroot/images/productos/{id} o wwwroot/images/servicios/{id}
                var uploadsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    baseFolder,
                    producto.Id.ToString()
                );

                Directory.CreateDirectory(uploadsPath); // crea carpeta si no existe

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Imagen.FileName)}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Imagen.CopyToAsync(stream);
                }

                var nuevaImagen = new Imagen
                {
                    Ruta = $"/images/{baseFolder}/{producto.Id}/{fileName}",
                    TipoImagen = "ProductoServicio",
                    IdRelacionado = producto.Id,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                };

                _context.Imagen.Add(nuevaImagen);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetProductoServicio), new { id = producto.Id }, producto);
        }

        // PUT: api/productosservicios/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Barbero")]
        public async Task<IActionResult> PutProductoServicio(
            int id,
            [FromForm] ProductoServicioConImagenDto dto
        )
        {
            if (id != dto.Id)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        message = "El ID en la URL no coincide con el ID del cuerpo de la solicitud.",
                    }
                );
            }

            var productoServicio = await _context.ProductosServicios.FindAsync(id);
            if (productoServicio == null)
            {
                return NotFound(
                    new { status = 404, message = "El producto o servicio no existe." }
                );
            }

            if (!(dto.EsAlmacenable ?? false) && dto.Cantidad > 0)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        message = "Un producto no almacenable no puede tener cantidad mayor a 0.",
                    }
                );
            }

            // Actualizar campos
            productoServicio.Nombre = dto.Nombre;
            productoServicio.Descripcion = dto.Descripcion;
            productoServicio.Precio = dto.Precio;
            productoServicio.EsAlmacenable = dto.EsAlmacenable;
            productoServicio.Cantidad = dto.Cantidad;

            // Manejo de imagen
            if (dto.Imagen != null && dto.Imagen.Length > 0)
            {
                // Buscar imagen existente en DB
                var imagenExistente = _context.Imagen.FirstOrDefault(i =>
                    i.TipoImagen == "ProductoServicio" && i.IdRelacionado == id && i.Activo == true
                );

                string baseFolder =
                    (productoServicio.EsAlmacenable ?? false) ? "productos" : "servicios";
                var carpetaPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    baseFolder,
                    id.ToString()
                );

                if (imagenExistente != null)
                {
                    // Borrar archivo físico anterior
                    var rutaFisicaAnterior = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        imagenExistente
                            .Ruta.TrimStart('/')
                            .Replace('/', Path.DirectorySeparatorChar)
                    );
                    if (System.IO.File.Exists(rutaFisicaAnterior))
                    {
                        System.IO.File.Delete(rutaFisicaAnterior);
                    }

                    // Actualizar ruta imagen
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Imagen.FileName)}";
                    var fullPath = Path.Combine(carpetaPath, fileName);
                    Directory.CreateDirectory(carpetaPath);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.Imagen.CopyToAsync(stream);
                    }

                    imagenExistente.Ruta = $"/images/{baseFolder}/{id}/{fileName}";
                    imagenExistente.FechaCreacion = DateTime.UtcNow;
                    _context.Imagen.Update(imagenExistente);
                }
                else
                {
                    // No había imagen previa: crear carpeta y guardar
                    Directory.CreateDirectory(carpetaPath);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Imagen.FileName)}";
                    var fullPath = Path.Combine(carpetaPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.Imagen.CopyToAsync(stream);
                    }

                    var nuevaImagen = new Imagen
                    {
                        Ruta = $"/images/{baseFolder}/{id}/{fileName}",
                        TipoImagen = "ProductoServicio",
                        IdRelacionado = id,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                    };

                    _context.Imagen.Add(nuevaImagen);
                }
            }

            _context.ProductosServicios.Update(productoServicio);
            await _context.SaveChangesAsync();

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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteProductoServicio(int id)
        {
            var productoServicio = await _context.ProductosServicios.FindAsync(id);
            if (productoServicio == null)
            {
                return NotFound(
                    new { status = 404, message = "El producto o servicio no existe." }
                );
            }

            var tieneDependencias = _context.DetalleAtencion.Any(d => d.ProductoServicioId == id);
            if (tieneDependencias)
            {
                return BadRequest(
                    new
                    {
                        status = 400,
                        message = "No se puede eliminar el producto porque está vinculado a una atención.",
                    }
                );
            }

            // Eliminar imagen/es relacionadas y archivos físicos
            var imagenes = _context
                .Imagen.Where(i => i.TipoImagen == "ProductoServicio" && i.IdRelacionado == id)
                .ToList();

            foreach (var imagen in imagenes)
            {
                var rutaFisica = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    imagen.Ruta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }
                _context.Imagen.Remove(imagen);
            }

            // Borrar carpeta del producto o servicio
            string baseFolder =
                (productoServicio.EsAlmacenable ?? false) ? "productos" : "servicios";
            var carpetaPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                baseFolder,
                id.ToString()
            );

            try
            {
                if (Directory.Exists(carpetaPath))
                {
                    Directory.Delete(carpetaPath, true); // elimina carpeta y todo su contenido
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        status = 500,
                        message = $"No se pudo eliminar la carpeta del producto: {ex.Message}",
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

        // DELETE: api/productosservicios/imagen/{idImagen}
        [HttpDelete("imagen/{idImagen}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteImagen(int idImagen)
        {
            var imagen = await _context.Imagen.FindAsync(idImagen);
            if (imagen == null)
            {
                return NotFound(new { status = 404, message = "La imagen no existe." });
            }

            var rutaFisica = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                imagen.Ruta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
            );
            if (System.IO.File.Exists(rutaFisica))
            {
                System.IO.File.Delete(rutaFisica);
            }

            _context.Imagen.Remove(imagen);
            await _context.SaveChangesAsync();

            // Verificar si la carpeta quedó vacía y eliminarla
            var carpetaPath = Path.GetDirectoryName(rutaFisica);
            if (
                Directory.Exists(carpetaPath)
                && !Directory.EnumerateFileSystemEntries(carpetaPath).Any()
            )
            {
                Directory.Delete(carpetaPath);
            }

            return Ok(new { status = 200, message = "Imagen eliminada correctamente." });
        }

        // GET: api/productosservicios/{id}/imagen
        [HttpGet("{id}/imagen")]
        [Authorize(Roles = "Administrador,Barbero")]
        public IActionResult GetImagen(int id)
        {
            var imagen = _context.Imagen.FirstOrDefault(i =>
                i.TipoImagen == "ProductoServicio" && i.IdRelacionado == id && i.Activo == true
            );

            if (imagen == null || string.IsNullOrEmpty(imagen.Ruta))
            {
                return NotFound(
                    new
                    {
                        status = 404,
                        message = "No hay imagen disponible para este producto o servicio.",
                    }
                );
            }

            return PhysicalFile(Path.Combine("wwwroot", imagen.Ruta.TrimStart('/')), "image/jpeg");
        }
    }
}

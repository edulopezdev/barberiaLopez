using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class DetalleAtencion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AtencionId { get; set; } // Clave foránea (Atencion)

        [Required]
        public int ProductoServicioId { get; set; } // Clave foránea (ProductoServicio)

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; } = 1; // Por defecto es 1

        [Required]
        [Range(0, 10000)]
        public decimal PrecioUnitario { get; set; } // Precio por unidad
    }
}
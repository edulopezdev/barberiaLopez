using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ProductoServicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Nombre { get; set; } // Nombre del producto o servicio

        public string? Descripcion { get; set; } // Descripción opcional (permite NULL)

        [Required]
        [Range(0, 10000)]
        public decimal? Precio { get; set; } // Precio del producto o servicio

        public bool? EsAlmacenable { get; set; } = false; // Indica si se puede almacenar (por defecto `false`)

        public int? Cantidad { get; set; } = 0; // Stock disponible, por defecto `0`
    }
}

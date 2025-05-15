using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Imagen
    {
        [Key]
        public int? IdImagen { get; set; } // Clave primaria (auto_increment)

        [Required]
        [MaxLength(255)]
        public string? Ruta { get; set; } // Ruta del archivo de imagen

        [Required]
        [Column(TypeName = "enum('AvatarUsuario','ProductoServicio','ComprobantePago')")]
        public string? TipoImagen { get; set; } // Tipo de imagen

        [Required]
        public int? IdRelacionado { get; set; } // Relación con la entidad correspondiente

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now; // Timestamp por defecto

        [Required]
        public bool Activo { get; set; } = true; // Indica si la imagen está activa
    }
}

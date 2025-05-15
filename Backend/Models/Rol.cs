using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? NombreRol { get; set; } // Nombre del rol (Ej: "Admin", "Empleado", "Cliente")
    }
}

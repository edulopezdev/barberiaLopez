using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Atencion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; } // Clave foránea (Usuario)

        [Required]
        public int BarberoId { get; set; } // Clave foránea (Usuario)

        [Required]
        public DateTime Fecha { get; set; } // Fecha de la atención

        [Required]
        [Range(0, 10000)]
        public decimal Total { get; set; } // Monto total de la atención

        public int? TurnoId { get; set; } // Relación opcional con Turno (puede ser NULL)
    }
}
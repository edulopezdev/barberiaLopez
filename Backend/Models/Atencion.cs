using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using backend.Data;
using backend.Models;

namespace backend.Models
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

        // Propiedad de navegación
        public Usuario Cliente { get; set; } = null!;
        public Usuario Barbero { get; set; } = null!;
        public ICollection<DetalleAtencion> DetalleAtencion { get; set; } =
            new List<DetalleAtencion>();
        public int? PagoId { get; set; }
        public Pago? Pago { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models; // Asegúrate de que Atencion esté en el mismo namespace

public enum MetodoPago
{
    Efectivo,
    TarjetaDebito,
    TarjetaCredito,
    Transferencia,
    MercadoPago,
    NaranjaX,
    QR,
    Otro,
}

public class Pago
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Atencion")] // Asegúrate de que el nombre del ForeignKey sea el correcto
    public int AtencionId { get; set; } // El ID de la atención, no la entidad completa

    [Required]
    public MetodoPago MetodoPago { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.Now;

    // Relación con la tabla Atencion, que puede ser opcional al momento de hacer el POST
    public virtual Atencion? Atencion { get; set; }
}

namespace backend.Dtos
{
    public class VentaDto
    {
        public int AtencionId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public DateTime FechaAtencion { get; set; }

        public List<DetalleVentaDto> Detalles { get; set; } = new();
        public decimal TotalVenta => Detalles.Sum(d => d.Subtotal);

        public PagoInfoDto? Pago { get; set; }
    }

    public class DetalleVentaDto
    {
        public int ProductoServicioId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class PagoInfoDto
    {
        public int PagoId { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
    }
}

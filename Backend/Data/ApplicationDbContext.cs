using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Mapeo de entidades con los nombres correctos según la base de datos
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<ProductoServicio> ProductosServicios { get; set; }
        public DbSet<Atencion> Atencion { get; set; }
        public DbSet<DetalleAtencion> DetalleAtencion { get; set; }
        public DbSet<EstadoTurno> EstadoTurno { get; set; }
        public DbSet<Imagen> Imagen { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Asegurar que cada entidad mapea correctamente a la tabla de la base de datos
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Turno>().ToTable("turno");
            modelBuilder.Entity<ProductoServicio>().ToTable("productos_servicios");
            modelBuilder.Entity<Atencion>().ToTable("atencion");
            modelBuilder.Entity<DetalleAtencion>().ToTable("detalle_atencion");
            modelBuilder.Entity<EstadoTurno>().ToTable("estado_turno");
            modelBuilder.Entity<Imagen>().ToTable("imagen");
            modelBuilder.Entity<Rol>().ToTable("rol");
            modelBuilder.Entity<Pago>().ToTable("pago");

            // Configuración de relaciones para `Pago`
            modelBuilder
                .Entity<Pago>()
                .HasOne(p => p.Atencion)
                .WithMany() // Una Atención puede tener múltiples pagos (pagos parciales)
                .HasForeignKey(p => p.AtencionId)
                .OnDelete(DeleteBehavior.Cascade); // Asegúrate de que esta política de eliminación es la que deseas

            // Configuración del Enum `MetodoPago` para almacenarse como `string`
            modelBuilder.Entity<Pago>().Property(p => p.MetodoPago).HasConversion<string>();
        }
    }
}

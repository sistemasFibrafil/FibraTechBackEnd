using Net.Business.Entities.Sap;
using Net.Business.Entities.Web;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.AppContext
{
    public class DataContextSeg : DbContext
    {
        public DataContextSeg(DbContextOptions<DataContextSeg> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ========================================================================================================================================================
            // // PERFIL (1:N) USUARIO
            // ========================================================================================================================================================
            modelBuilder.Entity<PerilEntity>(entity =>
            {
                entity.ToTable("Perfil", schema: "SEG");
                entity.HasKey(t => t.IdPerfil);

                entity.HasMany(p => p.Usuarios)
                      .WithOne(u => u.Perfil)
                      .HasForeignKey(u => u.IdPerfil)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // ========================================================================================================================================================
            // USUARIO
            // ========================================================================================================================================================
            modelBuilder.Entity<UsuarioEntity>(entity =>
            {
                entity.ToTable("Usuario", schema: "SEG");
                entity.HasKey(t => t.IdUsuario);
            });


            // ========================================================================================================================================================
            // Configuración LogisticUser y Persona (1:1)
            // ========================================================================================================================================================
            modelBuilder.Entity<LogisticUserEntity>(entity =>
            {
                entity.ToTable("LogisticUser", "SEG");
                entity.HasKey(t => t.IdLogisticUser);

                entity.Property(t => t.IdUsuario).IsRequired(false); // explícito: nullable relationship

                entity.HasOne(l => l.Usuario)
                      .WithOne(p => p.LogisticUser)        // enlaza con Usuario.LogisticUser
                      .HasForeignKey<LogisticUserEntity>(l => l.IdUsuario)
                      .IsRequired(false)                   // <= asegura relación opcional => LEFT JOIN
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.Permissions)
                      .WithOne(p => p.LogisticUser)
                      .HasForeignKey(p => p.IdLogisticUser)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // ========================================================================================================================================================
            // Configuración LogisticUserPermission (1:N)
            // ========================================================================================================================================================
            modelBuilder.Entity<LogisticUserPermissionEntity>(entity =>
            {
                entity.ToTable("LogisticUserPermission", "SEG");
                entity.HasKey(t => t.IdLogisticUserPermission);
            });


            // ========================================================================================================================================================
            // Configuración entre PickingInventario de productos terminados
            // ========================================================================================================================================================
            modelBuilder.Entity<TakeInventoryFinishedProductsEntity>(entity =>
            {
                entity.ToTable("TomaInventario", schema: "INV");

                entity.HasKey(t => t.DocEntry);

                entity.Property(t => t.IsDelete)
                    .ValueGeneratedOnAdd(); // (recomendada si el default está en SQL)
            });


            // ========================================================================================================================================================
            // Configuración entre TomaInventarioEntity y TomaInventario1Entity
            // ========================================================================================================================================================
            modelBuilder.Entity<TakeInventoryFinishedProducts1Entity>(entity =>
            {
                entity.ToTable("TomaInventario1", schema: "INV");

                // Clave primaria compuesta
                entity.HasKey(t => new { t.DocEntry, t.LineId });

                entity.Property(t => t.IsDelete)
                .ValueGeneratedOnAdd();   // (recomendada si el default está en SQL)
            });
            modelBuilder.Entity<TakeInventoryFinishedProducts1Entity>().HasOne(p => p.TakeInventoryFinishedProducts).WithMany(a => a.TakeInventoryFinishedProducts1).HasForeignKey(p => p.DocEntry).HasPrincipalKey(a => a.DocEntry);

        }


        public DbSet<PerilEntity> Peril { get; set; }
        public DbSet<UsuarioEntity> Usuario { get; set; }
        public DbSet<LogisticUserEntity> LogisticUser { get; set; }
        public DbSet<LogisticUserPermissionEntity> LogisticUserPermission { get; set; }
        public DbSet<TakeInventoryFinishedProductsEntity> TakeInventoryFinishedProducts { get; set; }
        public DbSet<TakeInventoryFinishedProducts1Entity> TakeInventoryFinishedProducts1 { get; set; }
    }
}

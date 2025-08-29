using Microsoft.EntityFrameworkCore;
using Net.Business.Entities;
using Net.Business.Entities.Sap;
namespace Net.Data.AppContext
{
    public class DataContextFil : DbContext
    {
        public DataContextFil(DbContextOptions<DataContextFil> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CampoDefinidoUsuarioEntity>().HasNoKey().ToTable("CUFD").HasKey(x=> new { x.TableID, x.FieldID });
            modelBuilder.Entity<CampoDefinidoUsuario1Entity>().HasNoKey().ToTable("UFD1").HasKey(x=> new { x.TableID, x.FieldID, x.IndexID });
            modelBuilder.Entity<CampoDefinidoUsuarioEntity>().HasMany(c=>c.Detalles).WithOne(o=>o.CampoDefinidoUsuario).HasForeignKey(x => new { x.TableID, x.FieldID });

            modelBuilder.Entity<TipoCambioSapEntity>().HasNoKey().ToTable("ORTT").HasKey(x=> new { x.RateDate, x.Currency });

            modelBuilder.Entity<OSKCViewEntity>().HasNoKey().ToView("SKU_VW_OSKC", "dbo");
            modelBuilder.Entity<OSKPViewEntity>().HasNoKey().ToView("SKU_VW_OSKP", "dbo");
            modelBuilder.Entity<SKP1Entity>().HasNoKey().ToTable("@FIB_SKP1").HasKey(x=> new { x.DocEntry, x.LineId});
        }


        /// <summary>
        /// DEFINICIONES
        /// </summary>
        public DbSet<ImpuestoSapEntity> Impuesto { get; set; }
        public DbSet<TipoCambioSapEntity> TipoCambio { get; set; }
        public DbSet<UnidadMedidaEntity> UnidadMedida { get; set; }
        public DbSet<LongitudAnchoEntity> LongitudAncho { get; set; }
        public DbSet<GrupoArticuloSapEntity> GrupoArticulo { get; set; }
        public DbSet<EmpleadoVentaSapEntity> EmpleadoVenta { get; set; }
        public DbSet<SubGrupoArticuloSapEntity> SubGrupoArticulo { get; set; }
        public DbSet<SubGrupoArticulo2SapEntity> SubGrupoArticulo2 { get; set; }


        public DbSet<CampoDefinidoUsuarioEntity> CampoDefinidoUsuario { get; set; }
        public DbSet<CampoDefinidoUsuario1Entity> CampoDefinidoUsuario1 { get; set; }
        public DbSet<TipoOperacionSapEntity> TipoOperacion { get; set; }
        public DbSet<TipoLaminadoEntity> TipoLaminado { get; set; }
        public DbSet<ColorImpresionEntity> ColorImpresion { get; set; }
        public DbSet<TiempoVidaEntity> TiempoVida { get; set; }
        public DbSet<ProcesoEntity> Proceso { get; set; }
        public DbSet<SocioNegocioSapEntity> OCRD { get; set; }
        public DbSet<OSKCEntity> OSKC { get; set; }
        public DbSet<ArticuloSapEntity> OITM { get; set; }
        public DbSet<OSKCViewEntity> OSKCView { get; set; }
        public DbSet<OSKPViewEntity> OSKPView { get; set; }
        public DbSet<SKP1Entity> SKP1 { get; set; }
    }
}

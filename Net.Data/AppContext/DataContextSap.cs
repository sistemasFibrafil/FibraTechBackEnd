using Net.Business.Entities;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.AppContext
{
    public class DataContextSap : DbContext
    {
        public DataContextSap(DbContextOptions<DataContextSap> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// HERRAMIENTAS
            /// </summary>
            // ========================================================================================================================================================
            // CONFIGURACIÓN GENERAL
            // ========================================================================================================================================================
            modelBuilder.Entity<GeneralSettingsEntity>().HasNoKey().ToTable("@FIB_CORE_OADM").HasKey(x => x.Code);

            // ========================================================================================================================================================
            // CAMPOS DEFINIDOS POR EL USUARIO
            // ========================================================================================================================================================
            modelBuilder.Entity<UserDefinedFieldsEntity>(e =>
            {
                e.ToTable("CUFD");
                e.HasKey(x => new { x.TableID, x.FieldID });

                e.HasMany(x => x.Lines)
                 .WithOne(x => x.CampoDefinidoUsuario)
                 .HasForeignKey(x => new { x.TableID, x.FieldID });
            });

            modelBuilder.Entity<UserDefinedFields1Entity>(e =>
            {
                e.ToTable("UFD1");
                e.HasKey(x => new { x.TableID, x.FieldID, x.IndexID });
            });







            /// <summary>
            /// GESTIÓN
            /// </summary>
            // ========================================================================================================================================================
            // TIPO DE CAMBIO
            // ========================================================================================================================================================
            modelBuilder.Entity<ExchangeRatesEntity>().HasNoKey().ToTable("ORTT").HasKey(x => new { x.RateDate, x.Currency });

            // ========================================================================================================================================================
            // EMPLEADOS DE VENTAS
            // ========================================================================================================================================================
            modelBuilder.Entity<SalesPersonsEntity>().HasNoKey().ToTable("OSLP").HasKey(x => x.SlpCode);





            /// <summary>
            /// INICIALIZACIÓN
            /// </summary>
            // ========================================================================================================================================================
            // DETALLE DE ADMINISTRACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<AdminInfoEntity>().ToTable("OADM").HasKey(x => x.Code);

            // ========================================================================================================================================================
            // NUMERACIÓN DOCUMENTO
            // ========================================================================================================================================================
            modelBuilder.Entity<NumeracionDocumentoEntity>().ToTable("ONNM").HasKey(x => new { x.ObjectCode, x.DocSubType });
            modelBuilder.Entity<NumeracionDocumento1Entity>().ToTable("NNM1").HasKey(x => new { x.ObjectCode, x.Series });
            modelBuilder.Entity<NumeracionDocumentoEntity>().HasOne(o => o.NumeracionDocumento1).WithOne().HasForeignKey<NumeracionDocumentoEntity>(o => new { o.ObjectCode, o.DfltSeries }).HasPrincipalKey<NumeracionDocumento1Entity>(n => new { n.ObjectCode, n.Series });

            // ========================================================================================================================================================
            // TIPO DOCUMENTO SUNAT
            // ========================================================================================================================================================
            modelBuilder.Entity<TipoDocumentoSunatEntity>().ToTable("@BPP_TPODOC").HasKey(x => x.Code);

            // ========================================================================================================================================================
            // NUMERACIÓN DOCUMENTO SUNAT
            // ========================================================================================================================================================
            modelBuilder.Entity<NumeracionDocumentoSunatEntity>().ToTable("@BPP_NUMDOC").HasKey(x => x.Code);

            



            /// <summary>
            /// DEFINICIONES
            /// </summary>
            modelBuilder.Entity<UsersEntity>().ToTable("OUSR").HasKey(x => x.USERID);
            modelBuilder.Entity<BranchesEntity>().ToTable("OUBR").HasKey(x => x.Code);
            modelBuilder.Entity<DepartmentsEntity>().ToTable("OUDP").HasKey(x => x.Code);

            // ========================================================================================================================================================
            // ALMACENES
            // ========================================================================================================================================================
            modelBuilder.Entity<WarehousesEntity>(entity =>
            {
                entity.ToTable("OWHS");
                entity.HasKey(e => e.WhsCode);

                // 🔹 Relación lógica OWHS.BalInvntAc → OACT.AcctCode (LEFT JOIN)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.BalInvntAc)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false);

                // Relación 1:N con ItemWarehouseInfo
                entity.HasMany(e => e.ItemWarehouseInfo)
                      .WithOne(iw => iw.Warehouses)
                      .HasForeignKey(iw => iw.WhsCode);
            });

            // ========================================================================================================================================================
            // ARTICULO - INFORMACIÓN DE ALMACÉN
            // ========================================================================================================================================================
            modelBuilder.Entity<ItemWarehouseInfoEntity>(entity =>
            {
                entity.ToTable("OITW");

                // 🔥 CLAVE COMPUESTA
                entity.HasKey(e => new { e.ItemCode, e.WhsCode });

                // Relación con Items
                entity.HasOne(e => e.Items)
                      .WithMany(i => i.ItemWarehouseInfo)
                      .HasForeignKey(e => e.ItemCode);

                // Relación con Warehouses
                entity.HasOne(e => e.Warehouses)
                      .WithMany(w => w.ItemWarehouseInfo)
                      .HasForeignKey(e => e.WhsCode);
            });
            modelBuilder.Entity<TipoOperacionEntity>().ToTable("@OK1_T12").HasKey(x => x.Code);
            modelBuilder.Entity<BusinessPartnerGroupsEntity>().HasNoKey().ToTable("OCRG").HasKey(x => x.GroupCode);

            // ========================================================================================================================================================
            // IMPUESTO
            // ========================================================================================================================================================
            modelBuilder.Entity<TaxGroupsEntity>(entity =>
            {
                entity.ToTable("OSTC");
                entity.HasKey(e => e.Code);
            });





            /// <summary>
            /// FINANZAS
            /// </summary>
            // ========================================================================================================================================================
            // PLAN DE CUENTAS
            // ========================================================================================================================================================
            modelBuilder.Entity<ChartOfAccountsEntity>(entity =>
            {
                entity.ToTable("OACT");
                entity.HasKey(e => e.AcctCode);
            });
            modelBuilder.Entity<CostCentersEntity>(entity =>
            {
                entity.ToTable("OOCR");
                entity.HasKey(e => e.OcrCode);
            });





            /// <summary>
            /// VENTAS
            /// </summary>
            // ========================================================================================================================================================
            // ORDEN DE VENTA
            // ========================================================================================================================================================
            modelBuilder.Entity<OrdersEntity>(entity =>
            {
                entity.ToTable("ORDR");

                entity.HasKey(e => e.DocEntry);

                // 1 → N con RDR1
                entity.HasMany(e => e.Lines)
                      .WithOne() // RDR1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<Orders1Entity>(entity =>
            {
                entity.ToTable("RDR1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);

                // RDR1 → Item (N → 1)
                entity.HasOne(e => e.Item)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.ItemCode)
                      .HasPrincipalKey(t => t.ItemCode)
                      .IsRequired(false); // LEFT JOIN

                // RDR1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.TipoOperacion)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN

                // RDR1 → ChartOfAccounts (N → 1)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.AcctCode)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false); // LEFT JOIN
            });





            /// <summary>
            /// COMPRAS
            /// </summary>
            // ========================================================================================================================================================
            // SOLICITUD DE COMPRA
            // ========================================================================================================================================================
            // ========================================================================================================================================================
            // VISTA SOCIO DE NEGOCIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<PurchaseRequestEntity>(entity =>
            {
                entity.ToTable("OPRQ");

                entity.HasKey(e => e.DocEntry);

                // 1 → N con PRQ1
                entity.HasMany(e => e.Lines)
                      .WithOne() // PRQ1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<PurchaseRequest1Entity>(entity =>
            {
                entity.ToTable("PRQ1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);

                // PRQ1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.TipoOperacion)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN

                // PRQ1 → ChartOfAccounts (N → 1) (LEFT JOIN)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.AcctCode)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false);
            });




            /// <summary>
            /// SOCIO DE NEGOCIOS
            /// </summary>
            // ========================================================================================================================================================
            // VISTA SOCIO DE NEGOCIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnersViewEntity>().HasNoKey().ToView("BP_VW_BusinessPartners", "dbo");





            /// <summary>
            /// INVENTARIO
            /// </summary>
            // ========================================================================================================================================================
            // VISTA REPORTE STOCK GENERAL
            // ========================================================================================================================================================
            modelBuilder.Entity<ItemsStockGeneralViewEntity>().HasNoKey().ToView("INV_VW_StockGeneral", "dbo");
            // ========================================================================================================================================================
            // PICKING
            // ========================================================================================================================================================
            modelBuilder.Entity<PickingEntity>().ToTable("@FIB_OPKG").HasKey(x => x.DocEntry);

            // ========================================================================================================================================================
            // ARTÍCULOS
            // ========================================================================================================================================================
            modelBuilder.Entity<ItemsEntity>(entity =>
            {
                entity.ToTable("OITM");
                entity.HasKey(e => e.ItemCode);

                // Relación 1:N con ItemWarehouseInfo
                entity.HasMany(e => e.ItemWarehouseInfo)
                      .WithOne(iw => iw.Items)
                      .HasForeignKey(iw => iw.ItemCode);

                // Relación N:1 (MUCHOS Items → UN Warehouse) - Un Item puede tener 0 o 1 almacén por defecto
                entity.HasOne(e => e.DefaultWarehouse)
                      .WithMany(w => w.DefaultItems)
                      .HasForeignKey(e => e.DfltWH)
                      .HasPrincipalKey(w => w.WhsCode)
                      .IsRequired(false)           // Item puede no tener almacén
                      .OnDelete(DeleteBehavior.NoAction);

                // Relación 1:N con PriceLists
                entity.HasMany(e => e.PriceLists)
                      .WithOne(p => p.Item)
                      .HasForeignKey(p => p.ItemCode);
            });

            // ========================================================================================================================================================
            // LISTA DE PRECIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<PriceListsEntity>(entity =>
            {
                entity.ToTable("ITM1");
                entity.HasKey(e => new { e.ItemCode, e.PriceList });
            });

            // ========================================================================================================================================================
            // ARTICULO - INFORMACIÓN DE ALMACÉN
            // ========================================================================================================================================================
            modelBuilder.Entity<ItemWarehouseInfoEntity>().ToTable("OITW").HasKey(x => new { x.ItemCode, x.WhsCode });

            // ========================================================================================================================================================
            // SOLICITUD DE TRASLADO
            // ========================================================================================================================================================
            modelBuilder.Entity<SolicitudTrasladoEntity>(entity =>
            {
                entity.ToTable("OWTQ");

                entity.HasKey(e => e.DocEntry);

                // 1 → N con WTQ1
                entity.HasMany(e => e.Lines)
                      .WithOne() // WTQ1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<SolicitudTraslado1Entity>(entity =>
            {
                entity.ToTable("WTQ1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                .IsUnicode(false);

                entity.HasOne(e => e.TipoOperacion)
                .WithMany() // ✅ MUCHOS a UNO
                .HasForeignKey(e => e.U_tipoOpT12)
                .HasPrincipalKey(t => t.Code)
                .IsRequired(false); // LEFT JOIN
            });

            // ========================================================================================================================================================
            // TRANSFERENCIA DE STOCK
            // ========================================================================================================================================================
            modelBuilder.Entity<TransferenciaStockEntity>(entity =>
            {
                entity.ToTable("OWTR");

                entity.HasKey(e => e.DocEntry);

                // 1 → N con WTR1
                entity.HasMany(e => e.Lines)
                      .WithOne() // WTR1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<TransferenciaStock1Entity>(entity =>
            {
                entity.ToTable("WTR1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);

                entity.HasOne(e => e.TipoOperacion)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN
            });

            // ========================================================================================================================================================
            // TOMA DE INVENTARIO DE REPUESTOS
            // ========================================================================================================================================================
            modelBuilder.Entity<TakeInventorySparePartsEntity>(entity =>
            {
                entity.ToTable("@FIB_OTIS");

                // Clave primaria
                entity.HasKey(x => x.DocEntry);
            });




            /// <summary>
            /// RECURSOS HUMANOS
            /// </summary>
            // ========================================================================================================================================================
            // EMPLEADOS
            // ========================================================================================================================================================
            modelBuilder.Entity<EmployeesInfoEntity>().HasNoKey().ToTable("OHEM").HasKey(x => x.empID);




            /// <summary>
            /// PRODUCCIÓN
            /// </summary>
            // ========================================================================================================================================================
            // SKU COMERCIAL
            // ========================================================================================================================================================
            modelBuilder.Entity<OSKCEntity>().ToTable("@FIB_OSKC").HasKey(x => x.Code);
            modelBuilder.Entity<OSKCViewEntity>().HasNoKey().ToView("SKU_VW_OSKC", "dbo");

            // ========================================================================================================================================================
            // SKU PRODUCCIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<SKP1Entity>().HasNoKey().ToTable("@FIB_SKP1").HasKey(x => new { x.DocEntry, x.LineId });
            modelBuilder.Entity<OSKPViewEntity>().HasNoKey().ToView("SKU_VW_OSKP", "dbo");
        }



        /// <summary>
        /// HERRAMIENTAS
        /// </summary>
        public DbSet<GeneralSettingsEntity> GeneralSettings { get; set; }
        public DbSet<UserDefinedFieldsEntity> UserDefinedFields { get; set; }
        public DbSet<UserDefinedFields1Entity> UserDefinedFields1 { get; set; }



        /// <summary>
        /// GESTIÓN
        /// </summary>
        public DbSet<ExchangeRatesEntity> ExchangeRates { get; set; }



        /// <summary>
        /// INICIALIZACIÓN
        /// </summary>
        public DbSet<AdminInfoEntity> AdminInfo { get; set; }
        public DbSet<NumeracionDocumentoEntity> NumeracionDocumento { get; set; }
        public DbSet<NumeracionDocumento1Entity> NumeracionDocumento1 { get; set; }
        public DbSet<NumeracionDocumentoSunatEntity> NumeracionDocumentoSunat { get; set; }



        /// <summary>
        /// DEFINICIONES
        /// </summary>
        public DbSet<UsersEntity> Users { get; set; }
        public DbSet<ProcesoEntity> Proceso { get; set; }
        public DbSet<BranchesEntity> Branches { get; set; }
        public DbSet<LocationEntity> Location { get; set; }
        public DbSet<TaxGroupsEntity> TaxGroups { get; set; }
        public DbSet<TiempoVidaEntity> TiempoVida { get; set; }
        public DbSet<WarehousesEntity> Warehouses { get; set; }
        public DbSet<ItemGroupsEntity> ItemGroups { get; set; }
        public DbSet<DepartmentsEntity> Departments { get; set; }
        public DbSet<SalesPersonsEntity> SalesPersons { get; set; }
        public DbSet<UnidadMedidaEntity> UnidadMedida { get; set; }
        public DbSet<TipoLaminadoEntity> TipoLaminado { get; set; }
        public DbSet<LongitudAnchoEntity> LongitudAncho { get; set; }
        public DbSet<CurrencyCodesEntity> CurrencyCodes { get; set; }
        public DbSet<TipoOperacionEntity> TipoOperacion { get; set; }
        public DbSet<ColorImpresionEntity> ColorImpresion { get; set; }
        public DbSet<PaymentTermsTypesEntity> PaymentTermsTypes { get; set; }
        public DbSet<SubGrupoArticuloSapEntity> SubGrupoArticulo { get; set; }
        public DbSet<TipoDocumentoSunatEntity> TipoDocumentoSunat { get; set; }
        public DbSet<SubGrupoArticulo2SapEntity> SubGrupoArticulo2 { get; set; }
        public DbSet<BusinessPartnerGroupsEntity> BusinessPartnerGroups { get; set; }




        /// <summary>
        /// FINANZAS
        /// </summary>
        public DbSet<ChartOfAccountsEntity> ChartOfAccounts { get; set; }
        public DbSet<CostCentersEntity> CostCenters { get; set; }




        /// <summary>
        /// VENTAS
        /// </summary>
        public DbSet<OrdersEntity> Orders { get; set; }





        /// <summary>
        /// COMPRAS
        /// </summary>
        public DbSet<PurchaseRequestEntity> PurchaseRequest { get; set; }




        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public DbSet<DireccionEntity> Direccion { get; set; }
        public DbSet<BusinessPartnersEntity> BusinessPartners { get; set; }
        public DbSet<PersonaContactoEntity> PersonaContacto { get; set; }
        public DbSet<BusinessPartnersViewEntity> BusinessPartnersView { get; set; }





        /// <summary>
        /// INVENTARIO
        /// </summary>
        public DbSet<ItemsEntity> Items { get; set; }
        public DbSet<PickingEntity> Picking { get; set; }
        public DbSet<PriceListsEntity> PriceLists { get; set; }
        public DbSet<ItemWarehouseInfoEntity> ItemWarehouseInfo { get; set; }
        public DbSet<CargaSaldoInicialEntity> CargaSaldoInicial { get; set; }
        public DbSet<SolicitudTrasladoEntity> SolicitudTraslado { get; set; }
        public DbSet<SolicitudTraslado1Entity> SolicitudTraslado1 { get; set; }
        public DbSet<TransferenciaStockEntity> TransferenciaStock { get; set; }
        public DbSet<TransferenciaStock1Entity> TransferenciaStock1 { get; set; }
        public DbSet<TakeInventorySparePartsEntity> TakeInventorySpares { get; set; }
        public DbSet<ItemsStockGeneralViewEntity> ItemsStockGeneralView { get; set; }




        /// <summary>
        /// PRODUCCIÓN
        /// </summary>
        public DbSet<OSKCEntity> OSKC { get; set; }
        public DbSet<SKP1Entity> SKP1 { get; set; }
        public DbSet<OSKCViewEntity> OSKCView { get; set; }
        public DbSet<OSKPViewEntity> OSKPView { get; set; }




        /// <summary>
        /// RECURSOS HUMANOS
        /// </summary>
        public DbSet<EmployeesInfoEntity> EmployeesInfo { get; set; }
    }
}

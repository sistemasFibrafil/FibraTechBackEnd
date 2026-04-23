using Microsoft.EntityFrameworkCore;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.AppContext
{
    public class DataContextSAPBusinessOne : DbContext
    {
        public DataContextSAPBusinessOne(DbContextOptions<DataContextSAPBusinessOne> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region <<< HERRAMIENTAS >>>

            /// <summary>
            /// HERRAMIENTAS
            /// </summary>
            // ========================================================================================================================================================
            // CONFIGURACIÓN GENERAL
            // ========================================================================================================================================================
            modelBuilder.Entity<GeneralSettingsEntity>(entity =>
            {
                entity.ToTable("@FIB_OADM");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // CAMPOS DEFINIDOS POR EL USUARIO
            // ========================================================================================================================================================
            modelBuilder.Entity<UserDefinedFieldsEntity>(e =>
            {
                e.ToTable("CUFD");
                e.HasKey(x => new { x.TableID, x.FieldID });

                e.HasMany(x => x.Lines)
                 .WithOne(x => x.UserDefinedFields)
                 .HasForeignKey(x => new { x.TableID, x.FieldID });
            });
            // ========================================================================================================================================================
            // DETALLE DE CAMPOS DEFINIDOS POR EL USUARIO
            // ========================================================================================================================================================
            modelBuilder.Entity<UserDefinedFields1Entity>(e =>
            {
                e.ToTable("UFD1");
                e.HasKey(x => new { x.TableID, x.FieldID, x.IndexID });
            });

            #endregion




            #region <<< GESTIÓN >>>

            /// <summary>
            /// GESTIÓN
            /// </summary>
            // ========================================================================================================================================================
            // TIPO DE CAMBIO
            // ========================================================================================================================================================
            modelBuilder.Entity<ExchangeRatesEntity>(entity =>
            {
                entity.ToTable("ORTT");
                entity.HasKey(e => new { e.RateDate, e.Currency });
            });

            #endregion




            #region <<< INICIALIZACIÓN >>>

            /// <summary>
            /// INICIALIZACIÓN
            /// </summary>
            // ========================================================================================================================================================
            // ADMINISTRACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<AdminInfoEntity>(entity =>
            {
                entity.ToTable("OADM");
                entity.HasKey(e => e.Code);

                entity.HasOne(e => e.AdminInfo1)
                      .WithOne(e => e.AdminInfo)
                      .HasForeignKey<AdminInfo1Entity>(e => e.Code);
            });
            // ========================================================================================================================================================
            // DETALLE DE ADMINISTRACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<AdminInfo1Entity>(entity =>
            {
                entity.ToTable("ADM1");
                entity.HasKey(e => e.Code);

                entity.HasOne(e => e.CountryEntity)
                      .WithMany(e => e.AdminInfos1)
                      .HasForeignKey(e => e.Country);
            });
            // ========================================================================================================================================================
            // CONFIGURACIÓN DE RUTA DE VÍAS DE ACCESO
            // ========================================================================================================================================================
            modelBuilder.Entity<AttachmentsSettingsEntity>(entity =>
            {
                entity.ToTable("OADP");
                entity.HasKey(e => e.PrintId);
            });
            // ========================================================================================================================================================
            // NUMERACIÓN DOCUMENTO
            // ========================================================================================================================================================
            modelBuilder.Entity<DocumentNumberingSeriesEntity>(entity =>
            {
                entity.ToTable("ONNM");
                entity.HasKey(e => new { e.ObjectCode, e.DocSubType });
            });
            // ========================================================================================================================================================
            // NUMERACIÓN DOCUMENTO DETALLE
            // ========================================================================================================================================================
            modelBuilder.Entity<DocumentNumberingSeries1Entity>(entity =>
            {
                entity.ToTable("NNM1");
                entity.HasKey(e => e.Series);
            });
            // ========================================================================================================================================================
            // TIPO DOCUMENTO SUNAT
            // ========================================================================================================================================================
            modelBuilder.Entity<DocumentTypeSunatEntity>(entity =>
            {
                entity.ToTable("@BPP_TPODOC");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // NUMERACIÓN DOCUMENTO SUNAT
            // ========================================================================================================================================================
            modelBuilder.Entity<DocumentNumberingSeriesSunatEntity>(entity =>
            {
                entity.ToTable("@BPP_NUMDOC");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // CABECERA DE CONFIGURACIÓN DE SERIES DE DOCUMENTOS
            // ========================================================================================================================================================
            modelBuilder.Entity<DocumentSeriesConfigurationEntity>(e =>
            {
                e.ToTable("@FIB_OCSD");
                e.HasKey(e => e.Code);

                // 🔗 ORDR → RDR1
                e.HasMany(e => e.Lines)
                      .WithOne()
                      .HasForeignKey(l => l.Code)
                      .IsRequired();
            });
            // ========================================================================================================================================================
            // DETALLE DE CONFIGURACIÓN DE SERIES DE DOCUMENTOS
            // ========================================================================================================================================================
            modelBuilder.Entity<DocumentSeriesConfiguration1Entity>(e =>
            {
                e.ToTable("@FIB_CSD1");
                e.HasKey(x => new { x.Code, x.LineId });
            });

            #endregion




            #region <<< DEFINICIONES >>>

            /// <summary>
            /// GENERAL
            /// </summary>
            // ========================================================================================================================================================
            // USUARIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<UsersEntity>(entity =>
            {
                entity.ToTable("OUSR");
                entity.HasKey(e => e.USERID);
            });
            // ========================================================================================================================================================
            // DEPARTAMENTOS / AREAS
            // ========================================================================================================================================================
            modelBuilder.Entity<DepartmentsEntity>(entity =>
            {
                entity.ToTable("OUDP");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // SUCURSALES
            // ========================================================================================================================================================
            modelBuilder.Entity<BranchesEntity>(entity =>
            {
                entity.ToTable("OUBR");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // EMPLEADOS DE VENTAS
            // ========================================================================================================================================================
            modelBuilder.Entity<SalesPersonsEntity>(entity =>
            {
                entity.ToTable("OSLP");
                entity.HasKey(e => e.SlpCode);
            });
            // ========================================================================================================================================================
            // TIPO DE OPERACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<OperationTypeEntity>(e =>
            {
                e.ToTable("@OK1_T12");
                e.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // PROCESOS
            // ========================================================================================================================================================
            modelBuilder.Entity<ProcessesEntity>(e =>
            {
                e.ToTable("@FIB_PROC");
                e.HasKey(e => e.Code);
            });




            /// <summary>
            /// FINANZAS
            /// </summary>
            // ========================================================================================================================================================
            // MONEDA
            // ========================================================================================================================================================
            modelBuilder.Entity<CurrencyCodesEntity>(entity =>
            {
                entity.ToTable("OCRN");
                entity.HasKey(e => e.CurrCode);
            });
            // ========================================================================================================================================================
            // IMPUESTO
            // ========================================================================================================================================================
            modelBuilder.Entity<TaxGroupsEntity>(entity =>
            {
                entity.ToTable("OSTC");
                entity.HasKey(e => e.Code);
            });



            /// <summary>
            /// SOCIO DE NEGOCIOS
            /// </summary>
            // ========================================================================================================================================================
            // GRUPOS DE SOCIOS DE NEGOCIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnerGroupsEntity>(entity =>
            {
                entity.ToTable("OCRG");
                entity.HasKey(e => e.GroupCode);
            });
            // ========================================================================================================================================================
            // PAISES
            // ========================================================================================================================================================
            modelBuilder.Entity<CountryEntity>(entity =>
            {
                entity.ToTable("OCRY");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // ESTADOS
            // ========================================================================================================================================================
            modelBuilder.Entity<StatesEntity>(entity =>
            {
                entity.ToTable("OCST");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // UBIGEO
            // ========================================================================================================================================================
            modelBuilder.Entity<UbigeoEntity>(entity =>
            {
                entity.ToTable("@FIB_UBIGEO");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // CONDICIONES DE PAGO
            // ========================================================================================================================================================
            modelBuilder.Entity<PaymentTermsTypesEntity>(entity =>
            {
                entity.ToTable("OCTG");
                entity.HasKey(e => e.GroupNum);
            });
            // ========================================================================================================================================================
            // SECTORES
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnerSectorsEntity>(entity =>
            {
                entity.ToTable("@FIB_SECTOR");
                entity.HasKey(e => e.Codigo);
            });



            /// <summary>
            /// INVENTARIO
            /// </summary>
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
            // GRUPO DE ARTÍCULOS
            // ========================================================================================================================================================
            modelBuilder.Entity<ItemGroupsEntity>(entity =>
            {
                entity.ToTable("OITB");
                entity.HasKey(e => e.ItmsGrpCod);
            });
            modelBuilder.Entity<SubGrupoArticuloEntity>(entity =>
            {
                entity.ToTable("FIB_SGRUPO");
                entity.HasKey(e => e.Code);
            });
            modelBuilder.Entity<SubGrupoArticulo2SapEntity>(entity =>
            {
                entity.ToTable("FIB_SGRUPO2");
                entity.HasKey(e => e.Code);
            });

            #endregion




            #region <<< PROCEDIMIENTO DE AUTORIZACIÓN >>>

            // ========================================================================================================================================================
            // MODELOS DE AUTORIZACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<ApprovalTemplatesEntity>(entity =>
            {
                entity.ToTable("OWTM");
                entity.HasKey(e => e.WtmCode);
            });

            // ========================================================================================================================================================
            // ETAPAS DE AUTORIZACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<ApprovalStagesEntity>(entity =>
            {
                entity.ToTable("OWST");
                entity.HasKey(e => e.WstCode);
            });

            // ========================================================================================================================================================
            // REQUERIMIENTOS DE APROBACIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<ApprovalRequestsEntity>(entity =>
            {
                entity.ToTable("OWDD");
                entity.HasKey(e => e.WddCode);


                // 🔗 OWDD → ODRF
                entity.HasOne(e => e.Drafts)
                      .WithMany() // ODRF no necesita colección
                      .HasForeignKey(e => e.DocEntry)
                      .HasPrincipalKey(b => b.DocEntry);


                // 🔗 OWDD → OUSR
                entity.HasOne(e => e.Users)
                      .WithMany() // OUSR no necesita colección
                      .HasForeignKey(e => e.OwnerID)
                      .HasPrincipalKey(b => b.USERID);


                // 🔗 OWDD → OWTM
                entity.HasOne(e => e.ApprovalTemplates)
                      .WithMany() // OWTM no necesita colección
                      .HasForeignKey(e => e.WtmCode)
                      .HasPrincipalKey(b => b.WtmCode);
            });
            modelBuilder.Entity<ApprovalRequestsLinesEntity>(entity =>
            {
                entity.ToTable("WDD1");

                entity.HasKey(e => new { e.WddCode, e.StepCode, e.UserID });
            });

            #endregion




            #region <<< FINANZAS >>>

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

            #endregion




            #region <<< DOCUMENTOS EN BORRADOR >>>

            // ========================================================================================================================================================
            // DOCUMENTOS DE BORRADOR
            // ========================================================================================================================================================
            modelBuilder.Entity<DraftsEntity>(entity =>
            {
                entity.ToTable("ODRF");

                entity.HasKey(e => e.DocEntry);

                // 🔗 ODRF → OCRD
                entity.HasOne(e => e.BusinessPartners)
                      .WithMany() // OCRD no necesita colección
                      .HasForeignKey(e => e.CardCode)
                      .HasPrincipalKey(b => b.CardCode);

                // 🔗 ODRF → OCRN
                entity.HasOne(e => e.CurrencyCodes)
                      .WithMany() // OCRN no necesita colección
                      .HasForeignKey(e => e.DocCur)
                      .HasPrincipalKey(b => b.CurrCode);

                // 🔗 ODRF → OSLP
                entity.HasOne(e => e.SalesPersons)
                      .WithMany() // OSLP no necesita colección
                      .HasForeignKey(e => e.SlpCode)
                      .HasPrincipalKey(b => b.SlpCode);

                // 🔗 ODRF → OCTG
                entity.HasOne(e => e.PaymentTermsTypes)
                      .WithMany() // OCTG no necesita colección
                      .HasForeignKey(e => e.GroupNum)
                      .HasPrincipalKey(b => b.GroupNum);

                // 🔗 ODRF → RDR1
                entity.HasMany(e => e.Lines)
                      .WithOne()
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<DraftsLinesEntity>(entity =>
            {
                entity.ToTable("DRF1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);


                // DRF1 → Item (N → 1)
                entity.HasOne(e => e.Item)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.ItemCode)
                      .HasPrincipalKey(t => t.ItemCode)
                      .IsRequired(false); // LEFT JOIN


                // DRF1 → ChartOfAccounts (N → 1)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.AcctCode)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false); // LEFT JOIN


                // DRF1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.OperationType)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN
            });

            #endregion




            #region <<< VENTAS >>>

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

                // 🔗 ORDR → OCRD
                entity.HasOne(e => e.BusinessPartners)
                      .WithMany() // OCRD no necesita colección
                      .HasForeignKey(e => e.CardCode)
                      .HasPrincipalKey(b => b.CardCode);

                // 🔗 ORDR → OCRN
                entity.HasOne(e => e.CurrencyCodes)
                      .WithMany() // OCRN no necesita colección
                      .HasForeignKey(e => e.DocCur)
                      .HasPrincipalKey(b => b.CurrCode);

                // 🔗 ORDR → OSLP
                entity.HasOne(e => e.SalesPersons)
                      .WithMany() // OSLP no necesita colección
                      .HasForeignKey(e => e.SlpCode)
                      .HasPrincipalKey(b => b.SlpCode);

                // 🔗 ORDR → OCTG
                entity.HasOne(e => e.PaymentTermsTypes)
                      .WithMany() // OCTG no necesita colección
                      .HasForeignKey(e => e.GroupNum)
                      .HasPrincipalKey(b => b.GroupNum);

                // 🔗 ORDR → RDR1
                entity.HasMany(e => e.Lines)
                      .WithOne()
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


                // RDR1 → ChartOfAccounts (N → 1)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.AcctCode)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false); // LEFT JOIN


                // RDR1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.OperationType)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN
            });
            // ========================================================================================================================================================
            // ENTREGA DE VENTA
            // ========================================================================================================================================================
            modelBuilder.Entity<DeliveryNotesEntity>(entity =>
            {
                entity.ToTable("ODLN");

                entity.HasKey(e => e.DocEntry);

                // 🔗 ODLN → OUSR
                entity.HasOne(e => e.Users)
                      .WithMany() // OUSR no necesita colección
                      .HasForeignKey(e => e.UserSign)
                      .HasPrincipalKey(b => b.USERID);


                // 🔗 ODLN → OCRD
                entity.HasOne(e => e.BusinessPartners)
                      .WithMany() // OCRD no necesita colección
                      .HasForeignKey(e => e.CardCode)
                      .HasPrincipalKey(b => b.CardCode);

                // 🔗 ODLN → OCRN
                entity.HasOne(e => e.CurrencyCodes)
                      .WithMany() // OCRN no necesita colección
                      .HasForeignKey(e => e.DocCur)
                      .HasPrincipalKey(b => b.CurrCode);

                // 🔗 ODLN → OSLP
                entity.HasOne(e => e.SalesPersons)
                      .WithMany() // OSLP no necesita colección
                      .HasForeignKey(e => e.SlpCode)
                      .HasPrincipalKey(b => b.SlpCode);

                // 🔗 ODLN → OCTG
                entity.HasOne(e => e.PaymentTermsTypes)
                      .WithMany() // OCTG no necesita colección
                      .HasForeignKey(e => e.GroupNum)
                      .HasPrincipalKey(b => b.GroupNum);

                // 1 → N con DLN1
                entity.HasMany(e => e.Lines)
                      .WithOne() // DLN1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<DeliveryNotes1Entity>(entity =>
            {
                entity.ToTable("DLN1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);

                // DLN1 → Item (N → 1)
                entity.HasOne(e => e.Item)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.ItemCode)
                      .HasPrincipalKey(t => t.ItemCode)
                      .IsRequired(false); // LEFT JOIN

                
                // DLN1 → ChartOfAccounts (N → 1)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.AcctCode)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false); // LEFT JOIN


                // DLN1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.OperationType)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN
            });
            // ========================================================================================================================================================
            // FACTURA DE VENTA
            // ========================================================================================================================================================
            modelBuilder.Entity<InvoicesEntity>(entity =>
            {
                entity.ToTable("OINV");

                entity.HasKey(e => e.DocEntry);

                // 🔗 OINV → OUSR
                entity.HasOne(e => e.Users)
                      .WithMany() // OUSR no necesita colección
                      .HasForeignKey(e => e.UserSign)
                      .HasPrincipalKey(b => b.USERID);


                // 🔗 OINV → OCRD
                entity.HasOne(e => e.BusinessPartners)
                      .WithMany() // OCRD no necesita colección
                      .HasForeignKey(e => e.CardCode)
                      .HasPrincipalKey(b => b.CardCode);

                // 🔗 OINV → OCRN
                entity.HasOne(e => e.CurrencyCodes)
                      .WithMany() // OCRN no necesita colección
                      .HasForeignKey(e => e.DocCur)
                      .HasPrincipalKey(b => b.CurrCode);

                // 🔗 OINV → OSLP
                entity.HasOne(e => e.SalesPersons)
                      .WithMany() // OSLP no necesita colección
                      .HasForeignKey(e => e.SlpCode)
                      .HasPrincipalKey(b => b.SlpCode);

                // 🔗 OINV → OCTG
                entity.HasOne(e => e.PaymentTermsTypes)
                      .WithMany() // OCTG no necesita colección
                      .HasForeignKey(e => e.GroupNum)
                      .HasPrincipalKey(b => b.GroupNum);

                // 1 → N con INV1
                entity.HasMany(e => e.Lines)
                      .WithOne() // INV1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<Invoices1Entity>(entity =>
            {
                entity.ToTable("INV1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);


                // INV1 → Item (N → 1)
                entity.HasOne(e => e.Item)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.ItemCode)
                      .HasPrincipalKey(t => t.ItemCode)
                      .IsRequired(false); // LEFT JOIN

                // INV1 → ChartOfAccounts (N → 1)
                entity.HasOne(e => e.ChartOfAccounts)
                      .WithMany()
                      .HasForeignKey(e => e.AcctCode)
                      .HasPrincipalKey(c => c.AcctCode)
                      .IsRequired(false); // LEFT JOIN

                // INV1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.OperationType)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.U_tipoOpT12)
                      .HasPrincipalKey(t => t.Code)
                      .IsRequired(false); // LEFT JOIN
            });

            #endregion




            #region <<< COMPRAS >>>

            /// <summary>
            /// COMPRAS
            /// </summary>
            // ========================================================================================================================================================
            // SOLICITUD DE COMPRA
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


                // INV1 → Item (N → 1)
                entity.HasOne(e => e.Item)
                      .WithMany() // ✅ MUCHOS a UNO
                      .HasForeignKey(e => e.ItemCode)
                      .HasPrincipalKey(t => t.ItemCode)
                      .IsRequired(false); // LEFT JOIN


                // PRQ1 → TipoOperacion (N → 1)
                entity.HasOne(e => e.OperationType)
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

            #endregion




            #region <<< SOCIO DE NEGOCIOS >>>

            /// <summary>
            /// SOCIO DE NEGOCIOS
            /// </summary>
            // ========================================================================================================================================================
            // DATOS MAESTROS SOCIO DE NEGOCIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnersEntity>(entity =>
            {
                entity.ToTable("OCRD");
                entity.HasKey(e => e.CardCode);

                // 🔗 OCRD → OCRG
                entity.HasOne(e => e.BusinessPartnerGroups)
                      .WithMany() // OCRG no necesita colección
                      .HasForeignKey(e => e.GroupCode)
                      .HasPrincipalKey(b => b.GroupCode);

                entity.HasOne(e => e.SalesPersons)
                      .WithMany() // OSLP no necesita colección
                      .HasForeignKey(e => e.SlpCode)
                      .HasPrincipalKey(b => b.SlpCode);

                // 🔗 ORDR → @BPP_VEHICU
                entity.HasMany(e => e.LinesVehicles)
                      .WithOne()
                      .HasForeignKey(l => l.U_FIB_COTR)
                      .HasPrincipalKey(b => b.CardCode)
                      .IsRequired(false);

                // 🔗 ORDR → @BPP_CONDUC
                entity.HasMany(e => e.LinesDrivers)
                      .WithOne()
                      .HasForeignKey(l => l.U_FIB_COTR)
                      .HasPrincipalKey(b => b.CardCode)
                      .IsRequired(false);
            });
            // ========================================================================================================================================================
            // PERSONA DE CONTACTO
            // ========================================================================================================================================================
            modelBuilder.Entity<ContactEmployeesEntity>(entity =>
            {
                entity.ToTable("OCPR");

                // Clave primaria (PK)
                entity.HasKey(e => e.CntctCode);

                // Clave alterna (UK) para poder relacionar por (CardCode, Name)
                entity.HasAlternateKey(e => new { e.CardCode, e.Name });
            });
            // ========================================================================================================================================================
            // SOCIO DE NEGOCIOS Y PERSONA DE CONTACTO (RELACIÓN 1:N)
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnersEntity>()
            .HasOne(p => p.ContactEmployees)
            .WithMany()
            .HasForeignKey(p => new { p.CardCode, p.CntctPrsn })
            .HasPrincipalKey(c => new { c.CardCode, c.Name })
            .IsRequired(false);
            // ========================================================================================================================================================
            // SOCIO DE NEGOCIOS Y ESTADO (RELACIÓN 1:N)
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnersEntity>()
            .HasOne(p => p.State)
            .WithMany()
            .HasForeignKey(p => p.State2)
            .HasPrincipalKey(s => s.Code)
            .IsRequired(false);
            // ========================================================================================================================================================
            // VISTA SOCIO DE NEGOCIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<BusinessPartnersViewEntity>().HasNoKey().ToView("BP_VW_BusinessPartners", "dbo");
            // ========================================================================================================================================================
            // DIRECCIONES SOCIO DE NEGOCIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<AddressesEntity>(entity =>
            {
                entity.ToTable("CRD1");
                entity.HasKey(e => new { e.Address, e.CardCode, e.AdresType });

                entity.HasOne(d => d.BusinessPartner)
                      .WithMany(p => p.LinesAddresses)
                      .HasForeignKey(d => d.CardCode)
                      .IsRequired(false);

                // 🔗 CRD1 → OCST
                entity.HasOne(e => e.States)
                      .WithMany()
                      .HasForeignKey(e => e.State)
                      .HasPrincipalKey(b => b.Code)
                      .IsRequired(false);

                // 🔗 CRD1 → OSTC
                entity.HasOne(d => d.TaxGroup)
                      .WithMany()
                      .HasForeignKey(d => d.TaxCode)
                      .IsRequired(false);
            });
            // ========================================================================================================================================================
            // VEHICULOS
            // ========================================================================================================================================================
            modelBuilder.Entity<VehiclesEntity>(entity =>
            {
                entity.ToTable("@BPP_VEHICU");
                entity.HasKey(e => e.Code);
            });
            // ========================================================================================================================================================
            // CONDUCTORES
            // ========================================================================================================================================================
            modelBuilder.Entity<DriversEntity>(entity =>
            {
                entity.ToTable("@BPP_CONDUC");
                entity.HasKey(e => e.Code);
            });

            #endregion




            #region <<< INVENTARIO >>>

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
            modelBuilder.Entity<PickingEntity>(entity =>
            {
                entity.ToTable("@FIB_OPKG");
                entity.HasKey(x => x.DocEntry);

                entity.HasIndex(x => new
                {
                    x.U_BaseEntry,
                    x.U_BaseType,
                    x.U_BaseLine
                });
            });

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
            // ========================================================================================================================================================
            // LISTA DE PRECIOS
            // ========================================================================================================================================================
            modelBuilder.Entity<PriceListsEntity>(entity =>
            {
                entity.ToTable("ITM1");
                entity.HasKey(e => new { e.ItemCode, e.PriceList });
            });
            // ========================================================================================================================================================
            // SOLICITUD DE TRASLADO
            // ========================================================================================================================================================
            modelBuilder.Entity<InventoryTransferRequestEntity>(entity =>
            {
                entity.ToTable("OWTQ");

                entity.HasKey(e => e.DocEntry);

                // 1 → N con WTQ1
                entity.HasMany(e => e.Lines)
                      .WithOne() // WTQ1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<InventoryTransferRequest1Entity>(entity =>
            {
                entity.ToTable("WTQ1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                .IsUnicode(false);

                entity.HasOne(e => e.OperationType)
                .WithMany() // ✅ MUCHOS a UNO
                .HasForeignKey(e => e.U_tipoOpT12)
                .HasPrincipalKey(t => t.Code)
                .IsRequired(false); // LEFT JOIN
            });

            // ========================================================================================================================================================
            // TRANSFERENCIA DE STOCK
            // ========================================================================================================================================================
            modelBuilder.Entity<StockTransfersEntity>(entity =>
            {
                entity.ToTable("OWTR");

                entity.HasKey(e => e.DocEntry);

                // 1 → N con WTR1
                entity.HasMany(e => e.Lines)
                      .WithOne() // WTR1 no tiene navegación a cabecera
                      .HasForeignKey(l => l.DocEntry)
                      .IsRequired();
            });
            modelBuilder.Entity<StockTransfers1Entity>(entity =>
            {
                entity.ToTable("WTR1");

                entity.HasKey(e => new { e.DocEntry, e.LineNum });

                entity.Property(e => e.U_tipoOpT12)
                      .IsUnicode(false);

                entity.HasOne(e => e.OperationType)
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
            /// PRODUCCIÓN
            /// </summary>
            // ========================================================================================================================================================
            // SKU COMERCIAL
            // ========================================================================================================================================================
            modelBuilder.Entity<OSKCEntity>(entity =>
            {
                entity.ToTable("@FIB_OSKC");
                entity.HasKey(e => e.Code);
            });

            modelBuilder.Entity<OSKCViewEntity>().HasNoKey().ToView("SKU_VW_OSKC", "dbo");
            // ========================================================================================================================================================
            // SKU PRODUCCIÓN
            // ========================================================================================================================================================
            modelBuilder.Entity<SKP1Entity>(entity =>
            {
                entity.ToTable("@FIB_SKP1");
                entity.HasKey(e => new { e.DocEntry, e.LineId });
            });
            modelBuilder.Entity<OSKPViewEntity>().HasNoKey().ToView("SKU_VW_OSKP", "dbo");

            // ========================================================================================================================================================
            // LISTA DE PRECIOS DEFINICIÓN (OPLN)
            // ========================================================================================================================================================
            modelBuilder.Entity<PriceListEntity>(entity =>
            {
                entity.ToTable("OPLN");
                entity.HasKey(e => e.PriceListNo);
            });

            // ========================================================================================================================================================
            // DIVISION (@FIB_DIVI)
            // ========================================================================================================================================================
            modelBuilder.Entity<DivisionEntity>(entity =>
            {
                entity.ToTable("@FIB_DIVISION");
                entity.HasKey(e => e.Codigo);
            });



            #endregion




            #region <<< RECURSOS HUMANOS >>>

            /// <summary>
            /// RECURSOS HUMANOS
            /// </summary>
            // ========================================================================================================================================================
            // EMPLEADOS
            // ========================================================================================================================================================
            modelBuilder.Entity<EmployeesInfoEntity>(entity =>
            {
                entity.ToTable("OHEM");
                entity.HasKey(e => e.empID);
            });

            #endregion




            #region <<< PRODUCCIÓN >>>
            
            

            #endregion
        }




        #region <<< HERRAMIENTAS >>>

        /// <summary>
        /// HERRAMIENTAS
        /// </summary>
        public DbSet<GeneralSettingsEntity> GeneralSettings { get; set; }
        public DbSet<UserDefinedFieldsEntity> UserDefinedFields { get; set; }
        public DbSet<UserDefinedFields1Entity> UserDefinedFields1 { get; set; }

        #endregion




        #region <<< GESTIÓN >>>

        /// <summary>
        /// GESTIÓN
        /// </summary>
        public DbSet<ExchangeRatesEntity> ExchangeRates { get; set; }

        #endregion




        #region <<< INICIALIZACIÓN >>>

        /// <summary>
        /// INICIALIZACIÓN
        /// </summary>
        public DbSet<AdminInfoEntity> AdminInfo { get; set; }
        public DbSet<DocumentTypeSunatEntity> TipoDocumentoSunat { get; set; }
        public DbSet<AttachmentsSettingsEntity> AttachmentsSettings { get; set; }
        public DbSet<DocumentNumberingSeriesEntity> DocumentNumberingSeries { get; set; }
        public DbSet<DocumentNumberingSeries1Entity> DocumentNumberingSeries1 { get; set; }
        public DbSet<DocumentSeriesConfigurationEntity> DocumentSeriesConfiguration { get; set; }
        public DbSet<DocumentSeriesConfiguration1Entity> DocumentSeriesConfiguration1 { get; set; }
        public DbSet<DocumentNumberingSeriesSunatEntity> DocumentNumberingSeriesSunat { get; set; }

        #endregion




        #region <<< DEFINICIONES >>>

        public DbSet<UsersEntity> Users { get; set; }
        public DbSet<BranchesEntity> Branches { get; set; }
        public DbSet<LocationEntity> Location { get; set; }
        public DbSet<ProcessesEntity> Processes { get; set; }
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
        public DbSet<OperationTypeEntity> OperationType { get; set; }
        public DbSet<ColorImpresionEntity> ColorImpresion { get; set; }
        public DbSet<SubGrupoArticuloEntity> SubGrupoArticulo { get; set; }
        public DbSet<PaymentTermsTypesEntity> PaymentTermsTypes { get; set; }
        public DbSet<SubGrupoArticulo2SapEntity> SubGrupoArticulo2 { get; set; }
        public DbSet<BusinessPartnerGroupsEntity> BusinessPartnerGroups { get; set; }

        #endregion




        #region <<< PROCEDIMIENTO DE AUTORIZACIÓN >>>

        public DbSet<ApprovalStagesEntity> ApprovalStages { get; set; }
        public DbSet<ApprovalRequestsEntity> ApprovalRequests { get; set; }
        public DbSet<ApprovalTemplatesEntity> ApprovalTemplates { get; set; }
        public DbSet<ApprovalRequestsLinesEntity> ApprovalRequestsLines { get; set; }

        #endregion




        #region <<< FINANZAS >>>

        /// <summary>
        /// FINANZAS
        /// </summary>
        public DbSet<CostCentersEntity> CostCenters { get; set; }
        public DbSet<ChartOfAccountsEntity> ChartOfAccounts { get; set; }

        #endregion




        #region <<< DOCUMENTOS DE BORRADOR >>>
        
        public DbSet<DraftsEntity> Drafts { get; set; }

        #endregion




        #region <<< VENTAS >>>

        /// <summary>
        /// VENTAS
        /// </summary>
        public DbSet<OrdersEntity> Orders { get; set; }
        public DbSet<Orders1Entity> Orders1 { get; set; }
        public DbSet<InvoicesEntity> Invoices { get; set; }
        public DbSet<Invoices1Entity> Invoices1 { get; set; }
        public DbSet<DeliveryNotesEntity> DeliveryNotes { get; set; }
        public DbSet<DeliveryNotes1Entity> DeliveryNotes1 { get; set; }

        #endregion




        #region <<< COMPRAS >>>

        /// <summary>
        /// COMPRAS
        /// </summary>
        public DbSet<PurchaseRequestEntity> PurchaseRequest { get; set; }

        #endregion




        #region <<< SOCIO DE NEGOCIOS >>>

        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public DbSet<DriversEntity> Driver { get; set; }
        public DbSet<VehiclesEntity> Vehicle { get; set; }
        public DbSet<AddressesEntity> Addresses { get; set; }
        public DbSet<BusinessPartnersEntity> BusinessPartners { get; set; }
        public DbSet<ContactEmployeesEntity> ContactEmployees { get; set; }
        public DbSet<BusinessPartnersViewEntity> BusinessPartnersView { get; set; }
        public DbSet<CountryEntity> Country { get; set; }
        public DbSet<StatesEntity> States { get; set; }
        public DbSet<UbigeoEntity> Ubigeo { get; set; }

        #endregion




        #region <<< INVENTARIO >>>

        /// <summary>
        /// INVENTARIO
        /// </summary>
        public DbSet<ItemsEntity> Items { get; set; }
        public DbSet<PickingEntity> Picking { get; set; }
        public DbSet<PriceListsEntity> PriceLists { get; set; }
        public DbSet<StockTransfersEntity> StockTransfers { get; set; }
        public DbSet<StockTransfers1Entity> StockTransfers1 { get; set; }
        public DbSet<ItemWarehouseInfoEntity> ItemWarehouseInfo { get; set; }
        public DbSet<CargaSaldoInicialEntity> CargaSaldoInicial { get; set; }
        public DbSet<ItemsStockGeneralViewEntity> ItemsStockGeneralView { get; set; }
        public DbSet<TakeInventorySparePartsEntity> TakeInventorySpareParts { get; set; }
        public DbSet<InventoryTransferRequestEntity> InventoryTransferRequest { get; set; }
        public DbSet<InventoryTransferRequest1Entity> InventoryTransferRequest1 { get; set; }

        #endregion




        #region <<< RECURSOS HUMANOS >>>

        /// <summary>
        /// RECURSOS HUMANOS
        /// </summary>
        public DbSet<EmployeesInfoEntity> EmployeesInfo { get; set; }

        #endregion




        #region <<< PRODUCCIÓN >>>

        /// <summary>
        /// PRODUCCIÓN
        /// </summary>
        public DbSet<OSKCEntity> OSKC { get; set; }
        public DbSet<SKP1Entity> SKP1 { get; set; }
        public DbSet<OSKCViewEntity> OSKCView { get; set; }
        public DbSet<OSKPViewEntity> OSKPView { get; set; }

        #endregion
        public DbSet<PriceListEntity> PriceList { get; set; }
        public DbSet<DivisionEntity> Division { get; set; }
        public DbSet<BusinessPartnerSectorsEntity> BusinessPartnerSectors { get; set; }
    }
}

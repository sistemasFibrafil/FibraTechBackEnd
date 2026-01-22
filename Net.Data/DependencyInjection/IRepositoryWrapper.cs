using Net.Data.Sap;
using Net.Data.Web;
namespace Net.Data
{
    public interface IRepositoryWrapper
    {
        // =================================================================
        // =================================================================
        // WEB
        // =================================================================
        // =================================================================

        /// <summary>
        /// SEGURIDAD
        /// </summary>
        IMenuRepository Menu { get; }
        IOpcionRepository Opcion { get; }
        IPerfilRepository Perfil { get; }
        IUsuarioRepository Usuario { get; }
        IDataBaseRepository DataBase { get; }
        IAuditoriaRepository Auditoria { get; }
        ILogisticUserRepository LogisticUser { get; }
        IOpcionxPerfilRepository OpcionxPerfil { get; }
        IParametroSistemaRepository ParametroSistema { get; }
        IParametroConexionRepository ParametroConexion { get; }

        /// <summary>
        /// GESTION
        /// </summary>
        IStatusRepository Status { get; }
        ITiempoRepository Tiempo { get; }
        
        /// <summary>
        /// VENTAS
        /// </summary>
        ISopRepository Sop { get; }
        IPickingListRepository PickingList { get; }
        IOrdenVentaSodimacRepository OrdenVentaSodimac { get; }





        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================

        /// <summary>
        /// GESTION
        /// </summary>
        IUsersRepository Users { get; }
        IProcesoRepository Proceso { get; }
        ILocationRepository Location { get; }
        IBranchesRepository Branches { get; }
        ITaxGroupsRepository TaxGroups { get; }
        IVehiculoSapRepository Vehiculo { get; }
        IWarehousesRepository Warehouses { get; }
        ITiempoVidaRepository TiempoVida { get; }
        IItemGroupsRepository ItemGroups { get; }
        IConductorSapRepository Conductor { get; }
        IDepartmentsRepository Departments { get; }
        IUnidadMedidaRepository UnidadMedida { get; }
        ITipoLaminadoRepository TipoLaminado { get; }
        ISalesPersonsRepository SalesPersons { get; }
        ICurrencyCodesRepository CurrencyCodes { get; }
        IExchangeRatesRepository ExchangeRates { get; }
        ILongitudAnchoRepository LongitudAncho { get; }
        IColorImpresionRepository ColorImpresion { get; }
        ITipoOperacionRepository TipoOperacion { get; }
        IUserDefinedFieldsRepository UserDefinedFields { get; }
        IPaymentTermsTypesRepository PaymentTermsTypes { get; }
        ISubGrupoArticuloSapRepository SubGrupoArticulo { get; }
        ITipoDocumentoSunatRepository TipoDocumentoSunat { get; }
        ISubGrupoArticulo2SapRepository SubGrupoArticulo2 { get; }
        INumeracionDocumentoRepository NumeracionDocumento { get; }
        IBusinessPartnerGroupsRepository BusinessPartnerGroups { get; }
        IBusinessPartnerSectorsRepository BusinessPartnerSectors { get; }
        INumeracionDocumentoSunatRepository NumeracionDocumentoSunat { get; }



        /// <summary>
        /// FINZAS
        /// </summary>
        ICostCentersRepository CostCenters { get; }
        IChartOfAccountsRepository ChartOfAccounts { get; }



        /// <summary>
        /// COMPRAS
        /// </summary>
        IPurchaseRequestRepository PurchaseRequest { get; }



        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        IDireccionRepository Direccion { get; }
        IBusinessPartnersRepository SocioNegocio { get; }
        IPersonaContactoSapRepository PersonaContacto { get; }


        /// <summary>
        /// VENTAS
        /// </summary>
        IOrdersRepository Orders { get; }
        IEntregaSapRepository Entrega { get; }
        IFacturaVentaSapRepository FacturaVenta { get; }
        IOSKCRepository OSKC { get; }
        IOSKPRepository OSKP { get; }



        /// <summary>
        /// FACTURACIÓN ELECTRÓNICA
        /// </summary>
        IGuiaElectronicaSapRepository GuiaElectronica { get; }
        IFacturacionElectronicaSapRepositoy FacturacionElectronica { get; }



        /// <summary>
        /// INVENTARIO
        /// </summary>
        IPickingRepository Picking { get; }
        IItemsRepository Articulo { get; }
        ICargaSaldoInicialRepository CargaSaldoInicial { get; }
        ISolicitudTrasladoRepository SolicitudTraslado { get; }
        ITransferenciaStockRepository TransferenciaStock { get; }
        ITakeInventorySparePartsRepository TakeInventorySpareParts { get; }
        ITakeInventoryFinishedProductsRepository TakeInventoryFinishedProducts { get; }
        


        /// <summary>
        /// GESTION DE BANCOS
        /// </summary>
        IPagoRecibidoRepository PagoRecibido { get; }



        /// <summary>
        /// PRODUCCION
        /// </summary>
        IOrdenFabricacionSapRepository OrdenFabricacion { get; }



        /// <summary>
        /// RECURSOS HUMANOS
        /// </summary>
        IEmployeesInfoRepository EmployeesInfo { get; }
    }
}

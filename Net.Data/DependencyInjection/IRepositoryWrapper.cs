using Net.Data.Web;
using Net.Data.SAPBusinessOne;
using Net.Data.SAPBusinessOne.Administration;
namespace Net.Data
{
    public interface IRepositoryWrapper
    {
        // =================================================================
        // =================================================================
        // WEB
        // =================================================================
        // =================================================================
        #region <<< << SEGURIDAD >>>
        ISopRepository Sop { get; }
        IMenuRepository Menu { get; }
        IOpcionRepository Opcion { get; }
        IPerfilRepository Perfil { get; }
        IUsuarioRepository Usuario { get; }
        IDataBaseRepository DataBase { get; }
        IAuditoriaRepository Auditoria { get; }
        IPickingListRepository PickingList { get; }
        ILogisticUserRepository LogisticUser { get; }
        IOpcionxPerfilRepository OpcionxPerfil { get; }
        IParametroSistemaRepository ParametroSistema { get; }
        IParametroConexionRepository ParametroConexion { get; }
        IOrdenVentaSodimacRepository OrdenVentaSodimac { get; }
        ITakeInventoryFinishedProductsRepository TakeInventoryFinishedProducts { get; }

        #endregion




        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================
        #region <<< HERRAMIENTAS >>>

        IUserDefinedFieldsRepository UserDefinedFields { get; }

        #endregion




        #region <<< GESTIÓN >>>

        IExchangeRatesRepository ExchangeRates { get; }

        #endregion




        #region <<< INICIALIZACIÓN >>>

        IDocumentTypeSunatRepository DocumentTypeSunat { get; }
        IDocumentNumberingSeriesRepository DocumentNumberingSeries { get; }
        IDocumentSeriesConfigurationRepository DocumentSeriesConfiguration { get; }
        IDocumentNumberingSeriesSunatRepository DocumentNumberingSeriesSunat { get; }

        #endregion




        #region <<< DEFINICIONES >>>

        IUsersRepository Users { get; }
        IStatusRepository Status { get; }
        ITiempoRepository Tiempo { get; }
        ILocationRepository Location { get; }
        IBranchesRepository Branches { get; }
        IProcessesRepository Processes { get; }
        ITaxGroupsRepository TaxGroups { get; }
        ITiempoVidaRepository TiempoVida { get; }
        IWarehousesRepository Warehouses { get; }
        IItemGroupsRepository ItemGroups { get; }
        IDepartmentsRepository Departments { get; }
        IUnidadMedidaRepository UnidadMedida { get; }
        ITipoLaminadoRepository TipoLaminado { get; }
        ISalesPersonsRepository SalesPersons { get; }
        IOperationTypeRepository OperationType { get; }
        ICurrencyCodesRepository CurrencyCodes { get; }
        ILongitudAnchoRepository LongitudAncho { get; }
        IColorImpresionRepository ColorImpresion { get; }
        ISubGrupoArticuloRepository SubGrupoArticulo { get; }
        IPaymentTermsTypesRepository PaymentTermsTypes { get; }
        ISubGrupoArticulo2Repository SubGrupoArticulo2 { get; }
        IBusinessPartnerGroupsRepository BusinessPartnerGroups { get; }
        IBusinessPartnerSectorsRepository BusinessPartnerSectors { get; }

        #endregion




        #region <<< PROCEDIMIENTO DE AUTORIZACIÓN >>>
        
        IApprovalRequestsRepository ApprovalRequests { get; }

        #endregion




        #region <<< FINANZAS >>>

        ICostCentersRepository CostCenters { get; }
        IChartOfAccountsRepository ChartOfAccounts { get; }

        #endregion




        #region <<< DOCUMENTOS EN BORRADOR >>>

        IDraftsRepository Drafts { get; }


        #endregion




        #region <<< VENTAS >>>

        IOrdersRepository Orders { get; }
        IInvoicesRepository Invoices { get; }
        IDeliveryNotesRepository DeliveryNotes { get; }
        IFacturaVentaSapRepository FacturaVenta { get; }
        IGuiaElectronicaRepository GuiaElectronica { get; }
        IFacturacionElectronicaRepositoy FacturacionElectronica { get; }

        #endregion




        #region <<< COMPRAS >>>

        IPurchaseRequestRepository PurchaseRequest { get; }

        #endregion




        #region <<< SOCIOS DE NEGOCIOS >>>

        IDriversRepository Drivers { get; }
        IVehiclesRepository Vehicles { get; }
        IAddressesRepository Addresses { get; }
        IBusinessPartnersRepository BusinessPartners { get; }
        IContactEmployeesRepository ContactEmployees { get; }

        #endregion




        #region <<< GESTION DE BANCOS >>>

        IPagoRecibidoRepository PagoRecibido { get; }

        #endregion




        #region <<< INVENTARIO >>>

        IOSKCRepository OSKC { get; }
        IOSKPRepository OSKP { get; }
        IItemsRepository Items { get; }
        IPickingRepository Picking { get; }
        IStockTransfersRepository StockTransfers { get; }
        ICargaSaldoInicialRepository CargaSaldoInicial { get; }
        ITakeInventorySparePartsRepository TakeInventorySpareParts { get; }
        IInventoryTransferRequestRepository InventoryTransferRequest { get; }

        #endregion




        #region <<< RECURSOS HUMANOS >>>

        IEmployeesInfoRepository EmployeesInfo { get; }

        #endregion




        #region <<< PRODUCCIÓN >>>

        IOrdenFabricacionSapRepository OrdenFabricacion { get; }

        #endregion
    }
}

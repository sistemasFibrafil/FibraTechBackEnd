using AutoMapper;
using Net.Data.Web;
using Net.Connection;
using System.Net.Http;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Data.SAPBusinessOne;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Net.Data.SAPBusinessOne.Administration;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Data.SAPBusinessOne.BusinessPartners.Ubigeo;
using Net.Data.SAPBusinessOne.Administration.Definitions.General.Departments;
using Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.States;
using Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.Countries;
using Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroupsUserTable;
namespace Net.Data
{
    public class RepositoryWrapper(IConnectionSQL repoContext, IOptions<ParametrosTokenConfig> tokenConfig, IConfiguration configuration, IHttpClientFactory clientFactory, DataContextSeguridad dbSeguridad, DataContextSAPBusinessOne dbSapBusinessOne, DataContextProfil dbProfil, IMapper mapper, CompanyProviderSAPBusinessOne companyProviderSap) : IRepositoryWrapper
    {
        private readonly IMapper _mapper = mapper;
        private readonly IConnectionSQL _repoContext = repoContext;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        private readonly DataContextProfil _dbProfil = dbProfil;
        private readonly DataContextSeguridad _dbSeguridad = dbSeguridad;
        private readonly DataContextSAPBusinessOne _dbSAPBusinessOne = dbSapBusinessOne;

        private readonly IOptions<ParametrosTokenConfig> _tokenConfig = tokenConfig;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap = companyProviderSap;


        // =================================================================
        // =================================================================
        // WEB
        // =================================================================
        // =================================================================
        #region <<< << SEGURIDAD >>>

        private ISopRepository _sop;
        private IMenuRepository _menu;
        private IOpcionRepository _opcion;
        private IPerfilRepository _perfil;
        private IUsuarioRepository _usuario;
        private IDataBaseRepository _dataBase;
        private IAuditoriaRepository _auditoria;
        private IPickingListRepository _pickingList;
        private ILogisticUserRepository _logisticUser;
        private IOpcionxPerfilRepository _opcionxPerfil;
        private IParametroSistemaRepository _parametroSistema;
        private IParametroConexionRepository _parametroConexion;
        private IOrdenVentaSodimacRepository _ordenVentaSodimac;
        private ITakeInventoryFinishedProductsRepository _takeInventoryFinishedProducts;

        #endregion




        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================
        #region <<< HERRAMIENTAS >>>

        private IUserDefinedFieldsRepository _userDefinedFields;

        #endregion




        #region <<< GESTIÓN >>>

        private IExchangeRatesRepository _exchangeRates;

        #endregion




        #region <<< INICIALIZACIÓN >>>

        private IDocumentTypeSunatRepository _tipoDocumentoSunat;
        private IDocumentNumberingSeriesRepository _numeracionDocumento;
        private IDocumentSeriesConfigurationRepository _documentSeriesConfiguration;
        private IDocumentNumberingSeriesSunatRepository _documentNumberingSeriesSunat;

        #endregion




        #region <<< DEFINICIONES >>>

        private IUsersRepository _users;
        private IStatusRepository _status;
        private ITiempoRepository _tiempo;
        private IStatesRepository _states;
        private IUbigeoRepository _ubigeo;
        private ILocationRepository _location;
        private IBranchesRepository _branches;
        private IProcessesRepository _proceso;
        private ITaxGroupsRepository _taxGroups;
        private ICountriesRepository _countries;
        private ITiempoVidaRepository _tiempoVida;
        private IWarehousesRepository _warehouses;
        private IItemGroupsRepository _itemGroups;
        private IDepartmentsRepository _departments;
        private IUnidadMedidaRepository _unidadMedida;
        private ITipoLaminadoRepository _tipoLaminado;
        private ISalesPersonsRepository _salesPersons;
        private IOperationsTypesRepository _operationsTypes;
        private ICurrencyCodesRepository _currencyCodes;
        private ILongitudAnchoRepository _longitudAncho;
        private IColorImpresionRepository _colorImpresion;
        private ISubGrupoArticuloRepository _subGrupoArticulo;
        private IPaymentTermsTypesRepository _paymentTermsTypes;
        private ISubGrupoArticulo2Repository _subGrupoArticulo2;
        private IBusinessPartnerGroupsRepository _businessPartnerGroups;
        private IBusinessPartnerSectorsRepository _businessPartnerSectors;
        private IBusinessPartnerDivisionsRepository _businessPartnerDivisions;
        private IBusinessPartnerGroupsUserTableRepository _businessPartnerGroupsUserTable;

        #endregion



        
        #region <<< PROCEDIMIENTO DE AUTORIZACIÓN >>>

        private IApprovalRequestsRepository _approvalRequests;

        #endregion




        #region <<< FINANZAS >>>

        private ICostCentersRepository _costCenters;
        private IChartOfAccountsRepository _chartOfAccounts;

        #endregion




        #region <<< DOCUMENTOS EN BORRADOR >>>

        private IDraftsRepository _drafts;

        #endregion




        #region <<< VENTAS >>>

        private IOrdersRepository _orders;
        private IInvoicesRepository _invoices;
        private IDeliveryNotesRepository _deliveryNotes;
        private IFacturaVentaSapRepository _facturaVenta;
        private IGuiaElectronicaRepository _guiaElectronica;
        private IFacturacionElectronicaRepositoy _facturacionElectronica;

        #endregion




        #region <<< COMPRAS >>>

        private IPurchaseRequestRepository _purchaseRequest;

        #endregion




        #region <<< SOCIOS DE NEGOCIOS >>>

        private IDriversRepository _drivers;
        private IVehiclesRepository _vehicles;
        private IAddressesRepository _addresses;
        private IBusinessPartnersRepository _businessPartners;
        private IContactEmployeesRepository _contactEmployees;

        #endregion




        #region <<< GESTION DE BANCOS >>>
        
        private IPagoRecibidoRepository _pagoRecibido;

        #endregion




        #region <<< INVENTARIO >>>

        private IOSKPRepository _OSKP;
        private IOSKCRepository _OSKC;
        private IItemsRepository _items;
        private IPickingRepository _picking;
        private IStockTransfersRepository _stockTransfers;
        private ICargaSaldoInicialRepository _cargaSaldoInicial;
        private ITakeInventorySparePartsRepository _takeInventorySpareParts;
        private IInventoryTransferRequestRepository _inventoryTransferRequest;
        private IPriceListRepository _priceList;

        #endregion




        #region <<< RECURSOS HUMANOS >>>

        private IEmployeesInfoRepository _employeesInfo;

        #endregion




        #region <<< PRODUCCIÓN >>>

        private IOrdenFabricacionSapRepository _ordenFabricacion;

        #endregion




        // =================================================================
        // =================================================================
        // WEB
        // =================================================================
        // =================================================================
        #region <<< << SEGURIDAD >>>

        public ISopRepository Sop
        {
            get
            {
                _sop ??= new SopRepository(_repoContext, _configuration);
                return _sop;
            }
        }
        public IMenuRepository Menu
        {
            get
            {
                _menu ??= new MenuRepository(_repoContext);
                return _menu;
            }
        }
        public IOpcionRepository Opcion
        {
            get
            {
                _opcion ??= new OpcionRepository(_repoContext);
                return _opcion;
            }
        }
        public IPerfilRepository Perfil
        {
            get
            {
                _perfil ??= new PerfilRepository(_repoContext, _dbSeguridad);
                return _perfil;
            }
        }
        public IUsuarioRepository Usuario
        {
            get
            {
                _usuario ??= new UsuarioRepository(_repoContext, _tokenConfig, _dbSAPBusinessOne, _dbSeguridad, _companyProviderSap, _mapper);
                return _usuario;
            }
        }
        public IDataBaseRepository DataBase
        {
            get
            {
                _dataBase ??= new DataBaseRepository(_repoContext);
                return _dataBase;
            }
        }
        public IAuditoriaRepository Auditoria
        {
            get
            {
                _auditoria ??= new AuditoriaRepository(_repoContext);
                return _auditoria;
            }
        }
        public IPickingListRepository PickingList
        {
            get
            {
                _pickingList ??= new PickingListRepository(_repoContext, _configuration);
                return _pickingList;
            }
        }
        public ILogisticUserRepository LogisticUser
        {
            get
            {
                _logisticUser ??= new LogisticUserRepository(_repoContext, _dbSeguridad, _mapper);
                return _logisticUser;
            }
        }
        public IOpcionxPerfilRepository OpcionxPerfil
        {
            get
            {
                _opcionxPerfil ??= new OpcionxPerfilRepository(_repoContext);
                return _opcionxPerfil;
            }
        }
        public IParametroSistemaRepository ParametroSistema
        {
            get
            {
                _parametroSistema ??= new ParametroSistemaRepository(_repoContext);
                return _parametroSistema;
            }
        }
        public IParametroConexionRepository ParametroConexion
        {
            get
            {
                _parametroConexion ??= new ParametroConexionRepository(_repoContext);
                return _parametroConexion;
            }
        }
        public IOrdenVentaSodimacRepository OrdenVentaSodimac
        {
            get
            {
                _ordenVentaSodimac ??= new OrdenVentaSodimacRepository(_repoContext, _configuration);
                return _ordenVentaSodimac;
            }
        }
        public ITakeInventoryFinishedProductsRepository TakeInventoryFinishedProducts
        {
            get
            {
                _takeInventoryFinishedProducts ??= new TakeInventoryFinishedProductsRepository(_repoContext, _configuration, _dbSeguridad, _dbSAPBusinessOne, _companyProviderSap);
                return _takeInventoryFinishedProducts;
            }
        }

        #endregion




        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================
        #region <<< HERRAMIENTAS >>>

        public IUserDefinedFieldsRepository UserDefinedFields
        {
            get
            {
                _userDefinedFields ??= new UserDefinedFieldsRepository(_repoContext, _dbSAPBusinessOne);
                return _userDefinedFields;
            }
        }

        #endregion




        #region <<< GESTIÓN >>>

        public IExchangeRatesRepository ExchangeRates
        {
            get
            {
                _exchangeRates ??= new ExchangeRatesRepository(_repoContext, _dbSAPBusinessOne);
                return _exchangeRates;
            }
        }

        #endregion




        #region <<< INICIALIZACIÓN >>>

        public IDocumentTypeSunatRepository DocumentTypeSunat
        {
            get
            {
                _tipoDocumentoSunat ??= new DocumentTypeSunatRepository(_repoContext, _dbSAPBusinessOne);
                return _tipoDocumentoSunat;
            }
        }
        public IDocumentNumberingSeriesRepository DocumentNumberingSeries
        {
            get
            {
                _numeracionDocumento ??= new DocumentNumberingSeriesRepository(_repoContext, _dbSAPBusinessOne);
                return _numeracionDocumento;
            }
        }
        public IDocumentSeriesConfigurationRepository DocumentSeriesConfiguration
        {
            get
            {
                _documentSeriesConfiguration ??= new DocumentSeriesConfigurationRepository(_repoContext, _dbSAPBusinessOne, _dbSeguridad, _companyProviderSap);
                return _documentSeriesConfiguration;
            }
        }
        public IDocumentNumberingSeriesSunatRepository DocumentNumberingSeriesSunat
        {
            get
            {
                _documentNumberingSeriesSunat ??= new DocumentNumberingSeriesSunatRepository(_repoContext, _dbSAPBusinessOne);
                return _documentNumberingSeriesSunat;
            }
        }
        


        #endregion




        #region <<< DEFINICIONES >>>

        public IUsersRepository Users
        {
            get
            {
                _users ??= new UsersRepository(_repoContext, _dbSAPBusinessOne);
                return _users;
            }
        }
        public IStatusRepository Status
        {
            get
            {
                _status ??= new StatusRepository(_repoContext);
                return _status;
            }
        }
        public ITiempoRepository Tiempo
        {
            get
            {
                _tiempo ??= new TiempoRepository(_repoContext);
                return _tiempo;
            }
        }
        public IStatesRepository States
        {
            get
            {
                _states ??= new StatesRepository(_repoContext, _dbSAPBusinessOne);
                return _states;
            }
        }
        public IUbigeoRepository Ubigeo
        {
            get
            {
                _ubigeo ??= new UbigeoRepository(_repoContext, _dbSAPBusinessOne);
                return _ubigeo;
            }
        }
        public ILocationRepository Location
        {
            get
            {
                _location ??= new LocationRepository(_repoContext, _dbSAPBusinessOne);
                return _location;
            }
        }
        public IBranchesRepository Branches
        {
            get
            {
                _branches ??= new BranchesRepository(_repoContext, _dbSAPBusinessOne);
                return _branches;
            }
        }
        public IProcessesRepository Processes
        {
            get
            {
                _proceso ??= new ProcesoRepository(_repoContext, _dbSAPBusinessOne);
                return _proceso;
            }
        }
        public ITaxGroupsRepository TaxGroups
        {
            get
            {
                _taxGroups ??= new TaxGroupsRepository(_repoContext, _dbSAPBusinessOne);
                return _taxGroups;
            }
        }
        public ICountriesRepository Countries
        {
            get
            {
                _countries ??= new CountriesRepository(_repoContext, _dbSAPBusinessOne);
                return _countries;
            }
        }
        public ITiempoVidaRepository TiempoVida
        {
            get
            {
                _tiempoVida ??= new TiempoVidaRepository(_repoContext, _dbSAPBusinessOne);
                return _tiempoVida;
            }
        }
        public IWarehousesRepository Warehouses
        {
            get
            {
                _warehouses ??= new WarehousesRepository(_repoContext, _dbSAPBusinessOne);
                return _warehouses;
            }
        }
        public IItemGroupsRepository ItemGroups
        {
            get
            {
                _itemGroups ??= new ItemGroupsRepository(_repoContext, _dbSAPBusinessOne);
                return _itemGroups;
            }
        }
        public IDepartmentsRepository Departments
        {
            get
            {
                _departments ??= new DepartmentsRepository(_repoContext, _dbSAPBusinessOne);
                return _departments;
            }
        }
        public IUnidadMedidaRepository UnidadMedida
        {
            get
            {
                _unidadMedida ??= new UnidadMedidaRepository(_repoContext, _dbSAPBusinessOne);
                return _unidadMedida;
            }
        }
        public ITipoLaminadoRepository TipoLaminado
        {
            get
            {
                _tipoLaminado ??= new TipoLaminadoRepository(_repoContext, _dbSAPBusinessOne);
                return _tipoLaminado;
            }
        }
        public ISalesPersonsRepository SalesPersons
        {
            get
            {
                _salesPersons ??= new SalesPersonsRepository(_repoContext, _dbSAPBusinessOne);
                return _salesPersons;
            }
        }
        public IOperationsTypesRepository OperationsTypes
        {
            get
            {
                _operationsTypes ??= new OperationsTypesRepository(_repoContext, _dbSAPBusinessOne);
                return _operationsTypes;
            }
        }
        public ICurrencyCodesRepository CurrencyCodes
        {
            get
            {
                _currencyCodes ??= new CurrencyCodesRepository(_repoContext, _dbSAPBusinessOne);
                return _currencyCodes;
            }
        }
        public ILongitudAnchoRepository LongitudAncho
        {
            get
            {
                _longitudAncho ??= new LongitudAnchoRepository(_repoContext, _dbSAPBusinessOne);
                return _longitudAncho;
            }
        }
        public IColorImpresionRepository ColorImpresion
        {
            get
            {
                _colorImpresion ??= new ColorImpresionRepository(_repoContext, _dbSAPBusinessOne);
                return _colorImpresion;
            }
        }
        public ISubGrupoArticuloRepository SubGrupoArticulo
        {
            get
            {
                _subGrupoArticulo ??= new SubGrupoArticuloRepository(_repoContext, _dbSAPBusinessOne);
                return _subGrupoArticulo;
            }
        }
        public IPaymentTermsTypesRepository PaymentTermsTypes
        {
            get
            {
                _paymentTermsTypes ??= new PaymentTermsTypesRepository(_repoContext, _dbSAPBusinessOne);
                return _paymentTermsTypes;
            }
        }
        public ISubGrupoArticulo2Repository SubGrupoArticulo2
        {
            get
            {
                _subGrupoArticulo2 ??= new SubGrupoArticulo2SapRepository(_repoContext, _dbSAPBusinessOne);
                return _subGrupoArticulo2;
            }
        }
        
        
        public IBusinessPartnerGroupsRepository BusinessPartnerGroups
        {
            get
            {
                _businessPartnerGroups ??= new BusinessPartnerGroupsRepository(_repoContext, _dbSAPBusinessOne);
                return _businessPartnerGroups;
            }
        }
        public IBusinessPartnerSectorsRepository BusinessPartnerSectors
        {
            get
            {
                _businessPartnerSectors ??= new BusinessPartnerSectorsRepository(_repoContext, _dbSAPBusinessOne);
                return _businessPartnerSectors;
            }
        }
        public IBusinessPartnerDivisionsRepository BusinessPartnerDivisions
        {
            get
            {
                _businessPartnerDivisions ??= new BusinessPartnerDivisionsRepository(_repoContext, _dbSAPBusinessOne);
                return _businessPartnerDivisions;
            }
        }
        public IBusinessPartnerGroupsUserTableRepository BusinessPartnerGroupsUserTable
        {
            get
            {
                _businessPartnerGroupsUserTable ??= new BusinessPartnerGroupsUserTableRepository(_repoContext, _dbSAPBusinessOne);
                return _businessPartnerGroupsUserTable;
            }
        }
        #endregion




        #region <<< PROCEDIMIENTO DE AUTORIZACIÓN >>>

        public IApprovalRequestsRepository ApprovalRequests
        {
            get
            {
                _approvalRequests ??= new ApprovalRequestsRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                return _approvalRequests;
            }
        }

        #endregion




        #region <<< FINANZAS >>>

        public ICostCentersRepository CostCenters
        {
            get
            {
                _costCenters ??= new CostCentersRepository(_repoContext, _dbSAPBusinessOne);
                return _costCenters;
            }
        }
        public IChartOfAccountsRepository ChartOfAccounts
        {
            get
            {
                _chartOfAccounts ??= new ChartOfAccountsRepository(_repoContext, _dbSAPBusinessOne);
                return _chartOfAccounts;
            }
        }

        #endregion




        #region <<< DOCUMENTOS EN BORRADOR >>>

        public IDraftsRepository Drafts
        {
            get
            {
                _drafts ??= new DraftsRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                return _drafts;
            }
        }

        #endregion




        #region <<< VENTAS >>>

        public IOrdersRepository Orders
        {
            get
            {
                _orders ??= new OrdersRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                return _orders;
            }
        }
        public IInvoicesRepository Invoices
        {
            get
            {
                _invoices ??= new InvoicesRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                return _invoices;
            }
        }
        public IDeliveryNotesRepository DeliveryNotes
        {
            get
            {
                _deliveryNotes ??= new DeliveryNotesRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                return _deliveryNotes;
            }
        }
        public IFacturaVentaSapRepository FacturaVenta
        {
            get
            {
                _facturaVenta ??= new FacturaVentaSapRepository(_repoContext, _configuration);
                return _facturaVenta;
            }
        }
        public IGuiaElectronicaRepository GuiaElectronica
        {
            get
            {
                _guiaElectronica ??= new GuiaElectronicaRepository(_repoContext, _configuration);
                return _guiaElectronica;
            }
        }
        public IFacturacionElectronicaRepositoy FacturacionElectronica
        {
            get
            {
                _facturacionElectronica ??= new FacturacionElectronicaRepositoy(_repoContext, _configuration);
                return _facturacionElectronica;
            }
        }

        #endregion




        #region <<< COMPRAS >>>
        public IPurchaseRequestRepository PurchaseRequest
        {
            get
            {
                _purchaseRequest ??= new PurchaseRequestRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                return _purchaseRequest;
            }
        }

        #endregion




        #region <<< SOCIOS DE NEGOCIOS >>>

        public IDriversRepository Drivers
        {
            get
            {
                _drivers ??= new DriversRepository(_repoContext, _dbSAPBusinessOne, _mapper);
                return _drivers;
            }
        }
        public IVehiclesRepository Vehicles
        {
            get
            {
                _vehicles ??= new VehiclesRepository(_repoContext, _dbSAPBusinessOne, _mapper);
                return _vehicles;
            }
        }
        public IAddressesRepository Addresses
        {
            get
            {
                _addresses ??= new AddressesRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                return _addresses;
            }
        }
        public IBusinessPartnersRepository BusinessPartners
        {
            get
            {
                _businessPartners ??= new BusinessPartnersRepository(_repoContext, _tokenConfig, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                return _businessPartners;
            }
        }
        public IContactEmployeesRepository ContactEmployees
        {
            get
            {
                _contactEmployees ??= new ContactEmployeesRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                return _contactEmployees;
            }
        }



        #endregion




        #region <<<< GESTION DE BANCOS >>>

        public IPagoRecibidoRepository PagoRecibido
        {
            get
            {
                _pagoRecibido ??= new PagoRecibidoRepository(_repoContext, _configuration);
                return _pagoRecibido;
            }
        }

        #endregion




        #region <<< INVENTARIO >>>

        public IOSKPRepository OSKP
        {
            get
            {
                _OSKP ??= new OSKPRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper, _companyProviderSap);
                return _OSKP;
            }
        }
        public IOSKCRepository OSKC
        {
            get
            {
                _OSKC ??= new OSKCRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper, _companyProviderSap);
                return _OSKC;
            }
        }
        public IItemsRepository Items
        {
            get
            {
                _items ??= new ItemsRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                return _items;
            }
        }
        public IPickingRepository Picking
        {
            get
            {
                _picking ??= new PickingRepository(_repoContext, _configuration, _dbSAPBusinessOne, _dbProfil, _companyProviderSap);
                return _picking;
            }
        }
        public IStockTransfersRepository StockTransfers
        {
            get
            {
                _stockTransfers ??= new StockTransfersRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper, _companyProviderSap);
                return _stockTransfers;
            }
        }
        public ICargaSaldoInicialRepository CargaSaldoInicial
        {
            get
            {
                _cargaSaldoInicial ??= new CargaSaldoInicialRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper);
                return _cargaSaldoInicial;
            }
        }
        public IInventoryTransferRequestRepository InventoryTransferRequest
        {
            get
            {
                _inventoryTransferRequest ??= new InventoryTransferRequestRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                return _inventoryTransferRequest;
            }
        }
        public ITakeInventorySparePartsRepository TakeInventorySpareParts
        {
            get
            {
                _takeInventorySpareParts ??= new TakeInventorySparePartsRepository(_repoContext, _configuration, _dbSeguridad, _dbSAPBusinessOne, _companyProviderSap);
                return _takeInventorySpareParts;
            }
        }
        public IPriceListRepository PriceList
        {
            get
            {
                _priceList ??= new PriceListRepository(_repoContext, _dbSAPBusinessOne);
                return _priceList;
            }
        }

        #endregion




        #region <<< RECURSOS HUMANOS >>>

        public IEmployeesInfoRepository EmployeesInfo
        {
            get
            {
                _employeesInfo ??= new EmployeesInfoRepository(_repoContext, _dbSAPBusinessOne);
                return _employeesInfo;
            }
        }

        #endregion




        #region <<< PRODUCCIÓN >>>        

        public IOrdenFabricacionSapRepository OrdenFabricacion
        {
            get
            {
                _ordenFabricacion ??= new OrdenFabricacionSapRepository(_repoContext, _configuration);
                return _ordenFabricacion;
            }
        }

        #endregion
    }
}

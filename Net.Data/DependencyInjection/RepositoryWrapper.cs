using AutoMapper;
using Net.Data.Web;
using Net.Connection;
using System.Net.Http;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Data.SAPBusinessOne;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Data.SAPBusinessOne.Administration;
namespace Net.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IMapper _mapper;
        private readonly IConnectionSQL _repoContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        private readonly DataContextProfil _dbProfil;
        private readonly DataContextSeguridad _dbSeguridad;
        private readonly DataContextSAPBusinessOne _dbSAPBusinessOne;

        private readonly IOptions<ParametrosTokenConfig> _tokenConfig;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;


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
        private ILocationRepository _location;
        private IBranchesRepository _branches;
        private IProcessesRepository _proceso;
        private ITaxGroupsRepository _taxGroups;
        private ITiempoVidaRepository _tiempoVida;
        private IWarehousesRepository _warehouses;
        private IItemGroupsRepository _itemGroups;
        private IDepartmentsRepository _departments;
        private IUnidadMedidaRepository _unidadMedida;
        private ITipoLaminadoRepository _tipoLaminado;
        private ISalesPersonsRepository _salesPersons;
        private IOperationTypeRepository _operationType;
        private ICurrencyCodesRepository _currencyCodes;
        private ILongitudAnchoRepository _longitudAncho;
        private IColorImpresionRepository _colorImpresion;
        private ISubGrupoArticuloRepository _subGrupoArticulo;
        private IPaymentTermsTypesRepository _paymentTermsTypes;
        private ISubGrupoArticulo2Repository _subGrupoArticulo2;
        private IBusinessPartnerGroupsRepository _businessPartnerGroups;
        private IBusinessPartnerSectorsRepository _businessPartnerSectors;

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

        #endregion




        #region <<< RECURSOS HUMANOS >>>

        private IEmployeesInfoRepository _employeesInfo;

        #endregion




        #region <<< PRODUCCIÓN >>>

        private IOrdenFabricacionSapRepository _ordenFabricacion;

        #endregion




        public RepositoryWrapper(IConnectionSQL repoContext, IOptions<ParametrosTokenConfig> tokenConfig, IConfiguration configuration, IHttpClientFactory clientFactory, DataContextSeguridad dbSeguridad, DataContextSAPBusinessOne dbSapBusinessOne, DataContextProfil dbProfil, IMapper mapper, CompanyProviderSAPBusinessOne companyProviderSap)
        {
            _mapper = mapper;

            _dbProfil = dbProfil;
            _dbSeguridad = dbSeguridad;
            _dbSAPBusinessOne = dbSapBusinessOne;

            _repoContext = repoContext;
            _configuration = configuration;
            _clientFactory = clientFactory;

            _tokenConfig = tokenConfig;
            _companyProviderSap = companyProviderSap;
        }




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
                if (_sop == null)
                {
                    _sop = new SopRepository(_repoContext, _configuration);
                }
                return _sop;
            }
        }
        public IMenuRepository Menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new MenuRepository(_repoContext);
                }
                return _menu;
            }
        }
        public IOpcionRepository Opcion
        {
            get
            {
                if (_opcion == null)
                {
                    _opcion = new OpcionRepository(_repoContext);
                }
                return _opcion;
            }
        }
        public IPerfilRepository Perfil
        {
            get
            {
                if (_perfil == null)
                {
                    _perfil = new PerfilRepository(_repoContext, _dbSeguridad);
                }
                return _perfil;
            }
        }
        public IUsuarioRepository Usuario
        {
            get
            {
                if (_usuario == null)
                {
                    _usuario = new UsuarioRepository(_repoContext, _configuration, _tokenConfig, _dbSAPBusinessOne, _dbSeguridad, _companyProviderSap);
                }
                return _usuario;
            }
        }
        public IDataBaseRepository DataBase
        {
            get
            {
                if (_dataBase == null)
                {
                    _dataBase = new DataBaseRepository(_repoContext);
                }
                return _dataBase;
            }
        }
        public IAuditoriaRepository Auditoria
        {
            get
            {
                if (_auditoria == null)
                {
                    _auditoria = new AuditoriaRepository(_repoContext);
                }
                return _auditoria;
            }
        }
        public IPickingListRepository PickingList
        {
            get
            {
                if (_pickingList == null)
                {
                    _pickingList = new PickingListRepository(_repoContext, _configuration);
                }
                return _pickingList;
            }
        }
        public ILogisticUserRepository LogisticUser
        {
            get
            {
                if (_logisticUser == null)
                {
                    _logisticUser = new LogisticUserRepository(_repoContext, _dbSeguridad, _mapper);
                }
                return _logisticUser;
            }
        }
        public IOpcionxPerfilRepository OpcionxPerfil
        {
            get
            {
                if (_opcionxPerfil == null)
                {
                    _opcionxPerfil = new OpcionxPerfilRepository(_repoContext);
                }
                return _opcionxPerfil;
            }
        }
        public IParametroSistemaRepository ParametroSistema
        {
            get
            {
                if (_parametroSistema == null)
                {
                    _parametroSistema = new ParametroSistemaRepository(_repoContext);
                }
                return _parametroSistema;
            }
        }
        public IParametroConexionRepository ParametroConexion
        {
            get
            {
                if (_parametroConexion == null)
                {
                    _parametroConexion = new ParametroConexionRepository(_repoContext);
                }
                return _parametroConexion;
            }
        }
        public IOrdenVentaSodimacRepository OrdenVentaSodimac
        {
            get
            {
                if (_ordenVentaSodimac == null)
                {
                    _ordenVentaSodimac = new OrdenVentaSodimacRepository(_repoContext, _configuration);
                }
                return _ordenVentaSodimac;
            }
        }
        public ITakeInventoryFinishedProductsRepository TakeInventoryFinishedProducts
        {
            get
            {
                if (_takeInventoryFinishedProducts == null)
                {
                    _takeInventoryFinishedProducts = new TakeInventoryFinishedProductsRepository(_repoContext, _configuration, _dbSeguridad, _dbSAPBusinessOne, _companyProviderSap);
                }
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
                if (_userDefinedFields == null)
                {
                    _userDefinedFields = new UserDefinedFieldsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _userDefinedFields;
            }
        }

        #endregion




        #region <<< GESTIÓN >>>

        public IExchangeRatesRepository ExchangeRates
        {
            get
            {
                if (_exchangeRates == null)
                {
                    _exchangeRates = new ExchangeRatesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _exchangeRates;
            }
        }

        #endregion




        #region <<< INICIALIZACIÓN >>>

        public IDocumentTypeSunatRepository DocumentTypeSunat
        {
            get
            {
                if (_tipoDocumentoSunat == null)
                {
                    _tipoDocumentoSunat = new DocumentTypeSunatRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _tipoDocumentoSunat;
            }
        }
        public IDocumentNumberingSeriesRepository DocumentNumberingSeries
        {
            get
            {
                if (_numeracionDocumento == null)
                {
                    _numeracionDocumento = new DocumentNumberingSeriesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _numeracionDocumento;
            }
        }
        public IDocumentSeriesConfigurationRepository DocumentSeriesConfiguration
        {
            get
            {
                if (_documentSeriesConfiguration == null)
                {
                    _documentSeriesConfiguration = new DocumentSeriesConfigurationRepository(_repoContext, _dbSAPBusinessOne, _dbSeguridad, _companyProviderSap);
                }
                return _documentSeriesConfiguration;
            }
        }
        public IDocumentNumberingSeriesSunatRepository DocumentNumberingSeriesSunat
        {
            get
            {
                if (_documentNumberingSeriesSunat == null)
                {
                    _documentNumberingSeriesSunat = new DocumentNumberingSeriesSunatRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _documentNumberingSeriesSunat;
            }
        }
        


        #endregion




        #region <<< DEFINICIONES >>>

        public IUsersRepository Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new UsersRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _users;
            }
        }
        public IStatusRepository Status
        {
            get
            {
                if (_status == null)
                {
                    _status = new StatusRepository(_repoContext);
                }
                return _status;
            }
        }
        public ITiempoRepository Tiempo
        {
            get
            {
                if (_tiempo == null)
                {
                    _tiempo = new TiempoRepository(_repoContext);
                }
                return _tiempo;
            }
        }
        public ILocationRepository Location
        {
            get
            {
                if (_location == null)
                {
                    _location = new LocationRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _location;
            }
        }
        public IBranchesRepository Branches
        {
            get
            {
                if (_branches == null)
                {
                    _branches = new BranchesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _branches;
            }
        }
        public IProcessesRepository Processes
        {
            get
            {
                if (_proceso == null)
                {
                    _proceso = new ProcesoRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _proceso;
            }
        }
        public ITaxGroupsRepository TaxGroups
        {
            get
            {
                if (_taxGroups == null)
                {
                    _taxGroups = new TaxGroupsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _taxGroups;
            }
        }
        public ITiempoVidaRepository TiempoVida
        {
            get
            {
                if (_tiempoVida == null)
                {
                    _tiempoVida = new TiempoVidaRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _tiempoVida;
            }
        }
        public IWarehousesRepository Warehouses
        {
            get
            {
                if (_warehouses == null)
                {
                    _warehouses = new WarehousesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _warehouses;
            }
        }
        public IItemGroupsRepository ItemGroups
        {
            get
            {
                if (_itemGroups == null)
                {
                    _itemGroups = new ItemGroupsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _itemGroups;
            }
        }
        public IDepartmentsRepository Departments
        {
            get
            {
                if (_departments == null)
                {
                    _departments = new DepartmentsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _departments;
            }
        }
        public IUnidadMedidaRepository UnidadMedida
        {
            get
            {
                if (_unidadMedida == null)
                {
                    _unidadMedida = new UnidadMedidaRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _unidadMedida;
            }
        }
        public ITipoLaminadoRepository TipoLaminado
        {
            get
            {
                if (_tipoLaminado == null)
                {
                    _tipoLaminado = new TipoLaminadoRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _tipoLaminado;
            }
        }
        public ISalesPersonsRepository SalesPersons
        {
            get
            {
                if (_salesPersons == null)
                {
                    _salesPersons = new SalesPersonsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _salesPersons;
            }
        }
        public IOperationTypeRepository OperationType
        {
            get
            {
                if (_operationType == null)
                {
                    _operationType = new OperationTypeRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _operationType;
            }
        }
        public ICurrencyCodesRepository CurrencyCodes
        {
            get
            {
                if (_currencyCodes == null)
                {
                    _currencyCodes = new CurrencyCodesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _currencyCodes;
            }
        }
        public ILongitudAnchoRepository LongitudAncho
        {
            get
            {
                if (_longitudAncho == null)
                {
                    _longitudAncho = new LongitudAnchoRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _longitudAncho;
            }
        }
        public IColorImpresionRepository ColorImpresion
        {
            get
            {
                if (_colorImpresion == null)
                {
                    _colorImpresion = new ColorImpresionRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _colorImpresion;
            }
        }
        public ISubGrupoArticuloRepository SubGrupoArticulo
        {
            get
            {
                if (_subGrupoArticulo == null)
                {
                    _subGrupoArticulo = new SubGrupoArticuloRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _subGrupoArticulo;
            }
        }
        public IPaymentTermsTypesRepository PaymentTermsTypes
        {
            get
            {
                if (_paymentTermsTypes == null)
                {
                    _paymentTermsTypes = new PaymentTermsTypesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _paymentTermsTypes;
            }
        }
        public ISubGrupoArticulo2Repository SubGrupoArticulo2
        {
            get
            {
                if (_subGrupoArticulo2 == null)
                {
                    _subGrupoArticulo2 = new SubGrupoArticulo2SapRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _subGrupoArticulo2;
            }
        }
        public IBusinessPartnerGroupsRepository BusinessPartnerGroups
        {
            get
            {
                if (_businessPartnerGroups == null)
                {
                    _businessPartnerGroups = new BusinessPartnerGroupsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _businessPartnerGroups;
            }
        }
        public IBusinessPartnerSectorsRepository BusinessPartnerSectors
        {
            get
            {
                if (_businessPartnerSectors == null)
                {
                    _businessPartnerSectors = new BusinessPartnerSectorsRepository(_repoContext, _configuration);
                }
                return _businessPartnerSectors;
            }
        }
        #endregion




        #region <<< PROCEDIMIENTO DE AUTORIZACIÓN >>>

        public IApprovalRequestsRepository ApprovalRequests
        {
            get
            {
                if (_approvalRequests == null)
                {
                    _approvalRequests = new ApprovalRequestsRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _approvalRequests;
            }
        }

        #endregion




        #region <<< FINANZAS >>>

        public ICostCentersRepository CostCenters
        {
            get
            {
                if (_costCenters == null)
                {
                    _costCenters = new CostCentersRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _costCenters;
            }
        }
        public IChartOfAccountsRepository ChartOfAccounts
        {
            get
            {
                if (_chartOfAccounts == null)
                {
                    _chartOfAccounts = new ChartOfAccountsRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _chartOfAccounts;
            }
        }

        #endregion




        #region <<< DOCUMENTOS EN BORRADOR >>>

        public IDraftsRepository Drafts
        {
            get
            {
                if (_drafts == null)
                {
                    _drafts = new DraftsRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _drafts;
            }
        }

        #endregion




        #region <<< VENTAS >>>

        public IOrdersRepository Orders
        {
            get
            {
                if (_orders == null)
                {
                    _orders = new OrdersRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _orders;
            }
        }
        public IInvoicesRepository Invoices
        {
            get
            {
                if (_invoices == null)
                {
                    _invoices = new InvoicesRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _invoices;
            }
        }
        public IDeliveryNotesRepository DeliveryNotes
        {
            get
            {
                if (_deliveryNotes == null)
                {
                    _deliveryNotes = new DeliveryNotesRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _deliveryNotes;
            }
        }
        public IFacturaVentaSapRepository FacturaVenta
        {
            get
            {
                if (_facturaVenta == null)
                {
                    _facturaVenta = new FacturaVentaSapRepository(_repoContext, _configuration);
                }
                return _facturaVenta;
            }
        }
        public IGuiaElectronicaRepository GuiaElectronica
        {
            get
            {
                if (_guiaElectronica == null)
                {
                    _guiaElectronica = new GuiaElectronicaRepository(_repoContext, _configuration);
                }
                return _guiaElectronica;
            }
        }
        public IFacturacionElectronicaRepositoy FacturacionElectronica
        {
            get
            {
                if (_facturacionElectronica == null)
                {
                    _facturacionElectronica = new FacturacionElectronicaRepositoy(_repoContext, _configuration);
                }
                return _facturacionElectronica;
            }
        }

        #endregion




        #region <<< COMPRAS >>>
        public IPurchaseRequestRepository PurchaseRequest
        {
            get
            {
                if (_purchaseRequest == null)
                {
                    _purchaseRequest = new PurchaseRequestRepository(_repoContext, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _purchaseRequest;
            }
        }

        #endregion




        #region <<< SOCIOS DE NEGOCIOS >>>

        public IDriversRepository Drivers
        {
            get
            {
                if (_drivers == null)
                {
                    _drivers = new DriversRepository(_repoContext, _dbSAPBusinessOne, _mapper);
                }
                return _drivers;
            }
        }
        public IVehiclesRepository Vehicles
        {
            get
            {
                if (_vehicles == null)
                {
                    _vehicles = new VehiclesRepository(_repoContext, _dbSAPBusinessOne, _mapper);
                }
                return _vehicles;
            }
        }
        public IAddressesRepository Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    _addresses = new AddressesRepository(_repoContext, _dbSAPBusinessOne);
                }
                return _addresses;
            }
        }
        public IBusinessPartnersRepository BusinessPartners
        {
            get
            {
                _businessPartners ??= new BusinessPartnersRepository(_repoContext, _configuration, _dbSAPBusinessOne);
                return _businessPartners;
            }
        }
        public IContactEmployeesRepository ContactEmployees
        {
            get
            {
                if (_contactEmployees == null)
                {
                    _contactEmployees = new ContactEmployeesRepository(_repoContext, _dbSAPBusinessOne);
                }
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
                if (_OSKP == null)
                {
                    _OSKP = new OSKPRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper, _companyProviderSap);
                }
                return _OSKP;
            }
        }
        public IOSKCRepository OSKC
        {
            get
            {
                if (_OSKC == null)
                {
                    _OSKC = new OSKCRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper, _companyProviderSap);
                }
                return _OSKC;
            }
        }
        public IItemsRepository Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ItemsRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _items;
            }
        }
        public IPickingRepository Picking
        {
            get
            {
                if (_picking == null)
                {
                    _picking = new PickingRepository(_repoContext, _configuration, _dbSAPBusinessOne, _dbProfil, _companyProviderSap);
                }
                return _picking;
            }
        }
        public IStockTransfersRepository StockTransfers
        {
            get
            {
                if (_stockTransfers == null)
                {
                    _stockTransfers = new StockTransfersRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper, _companyProviderSap);
                }
                return _stockTransfers;
            }
        }
        public ICargaSaldoInicialRepository CargaSaldoInicial
        {
            get
            {
                if (_cargaSaldoInicial == null)
                {
                    _cargaSaldoInicial = new CargaSaldoInicialRepository(_repoContext, _configuration, _dbSAPBusinessOne, _mapper);
                }
                return _cargaSaldoInicial;
            }
        }
        public IInventoryTransferRequestRepository InventoryTransferRequest
        {
            get
            {
                if (_inventoryTransferRequest == null)
                {
                    _inventoryTransferRequest = new InventoryTransferRequestRepository(_repoContext, _configuration, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _inventoryTransferRequest;
            }
        }
        public ITakeInventorySparePartsRepository TakeInventorySpareParts
        {
            get
            {
                if (_takeInventorySpareParts == null)
                {
                    _takeInventorySpareParts = new TakeInventorySparePartsRepository(_repoContext, _configuration, _dbSeguridad, _dbSAPBusinessOne, _companyProviderSap);
                }
                return _takeInventorySpareParts;
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
                if (_ordenFabricacion == null)
                {
                    _ordenFabricacion = new OrdenFabricacionSapRepository(_repoContext, _configuration);
                }
                return _ordenFabricacion;
            }
        }

        #endregion
    }
}

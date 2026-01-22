using AutoMapper;
using Net.Data.Sap;
using Net.Data.Web;
using Net.Connection;
using System.Net.Http;
using Net.CrossCotting;
using Net.Data.AppContext;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
namespace Net.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IMapper _mapper;
        private readonly DataContextSeg _dbSeg;
        private readonly DataContextSap _dbSap;
        private readonly DataContextProfil _dbProfil;
        private readonly IConnectionSQL _repoContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly CompanyProviderSap _companyProviderSap;
        private readonly IOptions<ParametrosTokenConfig> _tokenConfig;


        // =================================================================
        // =================================================================
        // WEB
        // =================================================================
        // =================================================================

        /// <summary>
        /// SEGURIDAD
        /// </summary>
        private IMenuRepository _menu;
        private IOpcionRepository _opcion;
        private IPerfilRepository _perfil;
        private IUsuarioRepository _usuario;
        private IDataBaseRepository _dataBase;
        private IAuditoriaRepository _auditoria;
        private ILogisticUserRepository _logisticUser;
        private IOpcionxPerfilRepository _opcionxPerfil;
        private IParametroSistemaRepository _parametroSistema;
        private IParametroConexionRepository _parametroConexion;


        /// <summary>
        /// GESTION
        /// </summary>
        private IStatusRepository _status;
        private ITiempoRepository _tiempo;


        /// <summary>
        /// VENTAS
        /// </summary>
        private ISopRepository _sop;
        private IOSKCRepository _OSKC;
        private IOSKPRepository _OSKP;
        private IPickingListRepository _pickingList;
        private IOrdenVentaSodimacRepository _ordenVentaSodimac;





        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================

        /// <summary>
        /// GESTION
        /// </summary>
        private IUsersRepository _users;
        private IProcesoRepository _proceso;
        private ILocationRepository _location;
        private IBranchesRepository _branches;
        private ITaxGroupsRepository _taxGroups;
        private IVehiculoSapRepository _vehiculo;
        private IWarehousesRepository _warehouses;
        private ITiempoVidaRepository _tiempoVida;
        private IItemGroupsRepository _itemGroups;
        private IConductorSapRepository _conductor;
        private IDepartmentsRepository _departments;
        private IUnidadMedidaRepository _unidadMedida;
        private ITipoLaminadoRepository _tipoLaminado;
        private ISalesPersonsRepository _SalesPersons;
        private ICurrencyCodesRepository _currencyCodes;
        private IExchangeRatesRepository _exchangeRates;
        private ILongitudAnchoRepository _longitudAncho;
        private IColorImpresionRepository _colorImpresion;
        private ITipoOperacionRepository _tipoOperacion;
        private IUserDefinedFieldsRepository _userDefinedFields;
        private IPaymentTermsTypesRepository _paymentTermsTypes;
        private ISubGrupoArticuloSapRepository _subGrupoArticulo;
        private ITipoDocumentoSunatRepository _tipoDocumentoSunat;
        private ISubGrupoArticulo2SapRepository _subGrupoArticulo2;
        private INumeracionDocumentoRepository _numeracionDocumento;
        private IBusinessPartnerGroupsRepository _businessPartnerGroups;
        private IBusinessPartnerSectorsRepository _businessPartnerSectors;
        private INumeracionDocumentoSunatRepository _numeracionDocumentoSunat;


        /// <summary>
        /// FINZAS
        /// </summary>
        private ICostCentersRepository _costCenters;
        private IChartOfAccountsRepository _chartOfAccounts;


        /// <summary>
        /// COMPRAS
        /// </summary>
        private IPurchaseRequestRepository _purchaseRequest;


        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        private IDireccionRepository _direccion;
        private IBusinessPartnersRepository _socioNegocio;
        private IPersonaContactoSapRepository _personaContacto;


        /// <summary>
        /// VENTAS
        /// </summary>
        private IOrdersRepository _orders;
        private IEntregaSapRepository _entrega;
        private IFacturaVentaSapRepository _facturaVenta;


        /// <summary>
        /// FACTURACIÓN ELECTRÓNICA
        /// </summary>
        IGuiaElectronicaSapRepository _guiaElectronica;
        IFacturacionElectronicaSapRepositoy _facturacionElectronica;


        /// <summary>
        /// INVENTARIO
        /// </summary>
        private IPickingRepository _picking;
        private IItemsRepository _articulo;
        private ICargaSaldoInicialRepository _cargaSaldoInicial;
        private ISolicitudTrasladoRepository _solicitudTraslado;
        private ITransferenciaStockRepository _transferenciaStock;
        private ITakeInventorySparePartsRepository _takeInventorySpareParts;
        private ITakeInventoryFinishedProductsRepository _takeInventoryFinishedProducts;


        /// <summary>
        /// GESTION DE BANCOS
        /// </summary>
        private IPagoRecibidoRepository _pagoRecibido;


        /// <summary>
        /// PRODUCCION
        /// </summary>
        private IOrdenFabricacionSapRepository _ordenFabricacion;



        /// <summary>
        /// RECURSOS HUMANOS
        /// </summary>
        private IEmployeesInfoRepository _employeesInfo;



        public RepositoryWrapper(IConnectionSQL repoContext, IOptions<ParametrosTokenConfig> tokenConfig, IConfiguration configuration, IHttpClientFactory clientFactory, DataContextSeg dbSeg, DataContextSap dbSap, DataContextProfil dbProfil, IMapper mapper, CompanyProviderSap companyProviderSap)
        {
            _mapper = mapper;
            _dbSeg = dbSeg;
            _dbSap = dbSap;
            _dbProfil = dbProfil;
            _repoContext = repoContext;
            _tokenConfig = tokenConfig;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _companyProviderSap = companyProviderSap;
        }





        // =================================================================
        // =================================================================
        // WEB
        // =================================================================
        // =================================================================

        /// <summary>
        /// SEGURIDAD
        /// </summary>
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
                    _perfil = new PerfilRepository(_repoContext, _dbSeg);
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
                    _usuario = new UsuarioRepository(_repoContext, _configuration, _tokenConfig, _dbSap, _dbSeg, _companyProviderSap);
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
        public ILogisticUserRepository LogisticUser
        {
            get
            {
                if (_logisticUser == null)
                {
                    _logisticUser = new LogisticUserRepository(_repoContext, _dbSeg, _mapper);
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

        /// <summary>
        /// GESTIÓN
        /// </summary>
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

        /// <summary>
        /// INVENTARIO
        /// </summary>
        public ISolicitudTrasladoRepository SolicitudTraslado
        {
            get
            {
                if (_solicitudTraslado == null)
                {
                    _solicitudTraslado = new SolicitudTrasladoRepository(_repoContext, _configuration, _dbSap, _companyProviderSap);
                }
                return _solicitudTraslado;
            }
        }
        /// <summary>
        /// VENTAS
        /// </summary>
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





        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================

        /// <summary>
        /// GESTION
        /// </summary>
        public IUsersRepository Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new UsersRepository(_repoContext, _dbSap);
                }
                return _users;
            }
        }
        public IProcesoRepository Proceso
        {
            get
            {
                if (_proceso == null)
                {
                    _proceso = new ProcesoRepository(_repoContext, _dbSap);
                }
                return _proceso;
            }
        }
        public ILocationRepository Location
        {
            get
            {
                if (_location == null)
                {
                    _location = new LocationRepository(_repoContext, _dbSap);
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
                    _branches = new BranchesRepository(_repoContext, _dbSap);
                }
                return _branches;
            }
        }
        public IItemGroupsRepository ItemGroups
        {
            get
            {
                if (_itemGroups == null)
                {
                    _itemGroups = new ItemGroupsRepository(_repoContext, _dbSap);
                }
                return _itemGroups;
            }
        }
        public ITiempoVidaRepository TiempoVida
        {
            get
            {
                if (_tiempoVida == null)
                {
                    _tiempoVida = new TiempoVidaRepository(_repoContext, _dbSap);
                }
                return _tiempoVida;
            }
        }
        public ITaxGroupsRepository TaxGroups
        {
            get
            {
                if (_taxGroups == null)
                {
                    _taxGroups = new TaxGroupsRepository(_repoContext, _dbSap);
                }
                return _taxGroups;
            }
        }
        public IVehiculoSapRepository Vehiculo
        {
            get
            {
                if (_vehiculo == null)
                {
                    _vehiculo = new VehiculoSapRepository(_repoContext, _configuration);
                }
                return _vehiculo;
            }
        }
        public IWarehousesRepository Warehouses
        {
            get
            {
                if (_warehouses == null)
                {
                    _warehouses = new WarehousesRepository(_repoContext, _dbSap);
                }
                return _warehouses;
            }
        }
        public IConductorSapRepository Conductor
        {
            get
            {
                if (_conductor == null)
                {
                    _conductor = new ConductorSapRepository(_repoContext, _configuration);
                }
                return _conductor;
            }
        }
        public IDepartmentsRepository Departments
        {
            get
            {
                if (_departments == null)
                {
                    _departments = new DepartmentsRepository(_repoContext, _dbSap);
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
                    _unidadMedida = new UnidadMedidaRepository(_repoContext, _dbSap);
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
                    _tipoLaminado = new TipoLaminadoRepository(_repoContext, _dbSap);
                }
                return _tipoLaminado;
            }
        }
        public ICurrencyCodesRepository CurrencyCodes
        {
            get
            {
                if (_currencyCodes == null)
                {
                    _currencyCodes = new CurrencyCodesRepository(_repoContext, _dbSap);
                }
                return _currencyCodes;
            }
        }
        public IExchangeRatesRepository ExchangeRates
        {
            get
            {
                if (_exchangeRates == null)
                {
                    _exchangeRates = new ExchangeRatesRepository(_repoContext, _dbSap);
                }
                return _exchangeRates;
            }
        }
        public ILongitudAnchoRepository LongitudAncho
        {
            get
            {
                if (_longitudAncho == null)
                {
                    _longitudAncho = new LongitudAnchoRepository(_repoContext, _dbSap);
                }
                return _longitudAncho;
            }
        }
        public ISalesPersonsRepository SalesPersons
        {
            get
            {
                if (_SalesPersons == null)
                {
                    _SalesPersons = new SalesPersonsRepository(_repoContext, _dbSap);
                }
                return _SalesPersons;
            }
        }
        public IColorImpresionRepository ColorImpresion
        {
            get
            {
                if (_colorImpresion == null)
                {
                    _colorImpresion = new ColorImpresionRepository(_repoContext, _dbSap);
                }
                return _colorImpresion;
            }
        }
        public ITipoOperacionRepository TipoOperacion
        {
            get
            {
                if (_tipoOperacion == null)
                {
                    _tipoOperacion = new TipoOperacionRepository(_repoContext, _dbSap);
                }
                return _tipoOperacion;
            }
        }
        public IUserDefinedFieldsRepository UserDefinedFields
        {
            get
            {
                if (_userDefinedFields == null)
                {
                    _userDefinedFields = new UserDefinedFieldsRepository(_repoContext, _dbSap);
                }
                return _userDefinedFields;
            }
        }
        public IPaymentTermsTypesRepository PaymentTermsTypes
        {
            get
            {
                if (_paymentTermsTypes == null)
                {
                    _paymentTermsTypes = new PaymentTermsTypesRepository(_repoContext, _dbSap);
                }
                return _paymentTermsTypes;
            }
        }
        public ISubGrupoArticuloSapRepository SubGrupoArticulo
        {
            get
            {
                if (_subGrupoArticulo == null)
                {
                    _subGrupoArticulo = new SubGrupoArticuloSapRepository(_repoContext, _dbSap);
                }
                return _subGrupoArticulo;
            }
        }
        public ITipoDocumentoSunatRepository TipoDocumentoSunat
        {
            get
            {
                if (_tipoDocumentoSunat == null)
                {
                    _tipoDocumentoSunat = new TipoDocumentoSunatRepository(_repoContext, _dbSap);
                }
                return _tipoDocumentoSunat;
            }
        }
        public ISubGrupoArticulo2SapRepository SubGrupoArticulo2
        {
            get
            {
                if (_subGrupoArticulo2 == null)
                {
                    _subGrupoArticulo2 = new SubGrupoArticulo2SapRepository(_repoContext, _dbSap);
                }
                return _subGrupoArticulo2;
            }
        }
        public INumeracionDocumentoRepository NumeracionDocumento
        {
            get
            {
                if (_numeracionDocumento == null)
                {
                    _numeracionDocumento = new NumeracionDocumentoRepository(_repoContext, _dbSap, _mapper);
                }
                return _numeracionDocumento;
            }
        }
        public IBusinessPartnerGroupsRepository BusinessPartnerGroups
        {
            get
            {
                if (_businessPartnerGroups == null)
                {
                    _businessPartnerGroups = new BusinessPartnerGroupsRepository(_repoContext, _dbSap);
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
        public INumeracionDocumentoSunatRepository NumeracionDocumentoSunat
        {
            get
            {
                if (_numeracionDocumentoSunat == null)
                {
                    _numeracionDocumentoSunat = new NumeracionDocumentoSunatRepository(_repoContext, _dbSap);
                }
                return _numeracionDocumentoSunat;
            }
        }



        /// <summary>
        /// FINANZAS
        /// </summary>
        public ICostCentersRepository CostCenters
        {
            get
            {
                if (_costCenters == null)
                {
                    _costCenters = new CostCentersRepository(_repoContext, _dbSap);
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
                    _chartOfAccounts = new ChartOfAccountsRepository(_repoContext, _dbSap);
                }
                return _chartOfAccounts;
            }
        }



        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        public IPurchaseRequestRepository PurchaseRequest
        {
            get
            {
                if (_purchaseRequest == null)
                {
                    _purchaseRequest = new PurchaseRequestRepository(_repoContext, _dbSap, _companyProviderSap);
                }
                return _purchaseRequest;
            }
        }


        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        public IDireccionRepository Direccion
        {
            get
            {
                if (_direccion == null)
                {
                    _direccion = new DireccionRepository(_repoContext, _dbSap);
                }
                return _direccion;
            }
        }
        public IBusinessPartnersRepository SocioNegocio
        {
            get
            {
                if (_socioNegocio == null)
                {
                    _socioNegocio = new BusinessPartnersRepository(_repoContext, _configuration, _dbSap);
                }
                return _socioNegocio;
            }
        }
        public IPersonaContactoSapRepository PersonaContacto
        {
            get
            {
                if (_personaContacto == null)
                {
                    _personaContacto = new PersonaContactoSapRepository(_repoContext, _configuration);
                }
                return _personaContacto;
            }
        }



        /// <summary>
        /// VENTAS
        /// </summary>

        public IOrdersRepository Orders
        {
            get
            {
                if (_orders == null)
                {
                    _orders = new OrdersRepository(_repoContext, _configuration, _dbSap, _companyProviderSap);
                }
                return _orders;
            }
        }
        public IEntregaSapRepository Entrega
        {
            get
            {
                if (_entrega == null)
                {
                    _entrega = new EntregaSapRepository(_repoContext, _configuration);
                }
                return _entrega;
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
        public IOSKCRepository OSKC
        {
            get
            {
                if (_OSKC == null)
                {
                    _OSKC = new OSKCRepository(_repoContext, _configuration, _dbSap, _mapper, _companyProviderSap);
                }
                return _OSKC;
            }
        }
        public IOSKPRepository OSKP
        {
            get
            {
                if (_OSKP == null)
                {
                    _OSKP = new OSKPRepository(_repoContext, _configuration, _dbSap, _mapper, _companyProviderSap);
                }
                return _OSKP;
            }
        }



        /// <summary>
        /// FACTURACIÓN ELECTRÓNICA
        /// </summary>
        public IFacturacionElectronicaSapRepositoy FacturacionElectronica
        {
            get
            {
                if (_facturacionElectronica == null)
                {
                    _facturacionElectronica = new FacturacionElectronicaSapRepositoy(_repoContext, _configuration);
                }
                return _facturacionElectronica;
            }
        }
        public IGuiaElectronicaSapRepository GuiaElectronica
        {
            get
            {
                if (_guiaElectronica == null)
                {
                    _guiaElectronica = new GuiaElectronicaSapRepository(_repoContext, _configuration);
                }
                return _guiaElectronica;
            }
        }



        /// <summary>
        /// INVENTARIO
        /// </summary>
        public IPickingRepository Picking
        {
            get
            {
                if (_picking == null)
                {
                    _picking = new PickingRepository(_repoContext, _configuration, _dbSap, _dbProfil, _companyProviderSap);
                }
                return _picking;
            }
        }
        public IItemsRepository Articulo
        {
            get
            {
                if (_articulo == null)
                {
                    _articulo = new ItemsRepository(_repoContext, _configuration, _dbSap, _companyProviderSap);
                }
                return _articulo;
            }
        }
        public ICargaSaldoInicialRepository CargaSaldoInicial
        {
            get
            {
                if (_cargaSaldoInicial == null)
                {
                    _cargaSaldoInicial = new CargaSaldoInicialRepository(_repoContext, _configuration, _dbSap, _mapper);
                }
                return _cargaSaldoInicial;
            }
        }
        public ITransferenciaStockRepository TransferenciaStock
        {
            get
            {
                if (_transferenciaStock == null)
                {
                    _transferenciaStock = new TransferenciaStockRepository(_repoContext, _configuration, _dbSap, _mapper, _companyProviderSap);
                }
                return _transferenciaStock;
            }
        }
        public ITakeInventorySparePartsRepository TakeInventorySpareParts
        {
            get
            {
                if (_takeInventorySpareParts == null)
                {
                    _takeInventorySpareParts = new TakeInventorySparePartsRepository(_repoContext, _configuration, _dbSeg, _dbSap, _companyProviderSap);
                }
                return _takeInventorySpareParts;
            }
        }
        public ITakeInventoryFinishedProductsRepository TakeInventoryFinishedProducts
        {
            get
            {
                if (_takeInventoryFinishedProducts == null)
                {
                    _takeInventoryFinishedProducts = new TakeInventoryFinishedProductsRepository(_repoContext, _configuration, _dbSeg, _dbSap, _companyProviderSap);
                }
                return _takeInventoryFinishedProducts;
            }
        }



        /// <summary>
        /// GESTION DE BANCOS
        /// </summary>
        public IPagoRecibidoRepository PagoRecibido
        {
            get
            {
                if (_pagoRecibido == null)
                {
                    _pagoRecibido = new PagoRecibidoRepository(_repoContext, _configuration);
                }
                return _pagoRecibido;
            }
        }



        /// <summary>
        /// PRODUCCION
        /// </summary>
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




        /// <summary>
        /// PRODUCCION
        /// </summary>
        public IEmployeesInfoRepository EmployeesInfo
        {
            get
            {
                if (_employeesInfo == null)
                {
                    _employeesInfo = new EmployeesInfoRepository(_repoContext, _dbSap);
                }
                return _employeesInfo;
            }
        }
    }
}

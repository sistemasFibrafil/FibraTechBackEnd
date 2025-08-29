using System.Net.Http;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Data.Sap;
using Net.Data.Web;
namespace Net.Data
{
    public class RepositoryWrapper: IRepositoryWrapper
    {
        private readonly IMapper _am;
        private readonly DataContextFil _dc;
        private readonly IConnectionSQL _repoContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
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
        private IPersonaRepository _persona;
        private IDataBaseRepository _dataBase;
        private IAuditoriaRepository _auditoria;
        private IOpcionxPerfilRepository _opcionxPerfil;
        private IParametroSistemaRepository _parametroSistema;
        private IParametroConexionRepository _parametroConexion;

        /// <summary>
        /// GESTION
        /// </summary>
        private ISedeRepository _sede;
        private IStatusRepository _status;
        private ITiempoRepository _tiempo;
        private IFormularioRepository _formulario;
        private ITipoDocumentoRepository _tipoDocumento;
        private ISerieNumeracionRepository _serieNumeracion;

        /// <summary>
        /// INVENTARIO
        /// </summary>
        private ILecturaRepository _lectura;
        private IDocumentoLecturaRepository _documentoLectura;
        private ISolicitudTrasladoRepository _solicitudTraslado;
        private ITransferenciaStockRepositoy _transferenciaStock;

        /// <summary>
        /// VENTAS
        /// </summary>
        private ISopRepository _sop;
        private IPickingListRepository _pickingList;
        private IOrdenVentaSodimacRepository _ordenVentaSodimac;
        private IOSKCRepository _OSKC;
        private IOSKPRepository _OSKP;





        // =================================================================
        // =================================================================
        // SAP Business One
        // =================================================================
        // =================================================================

        /// <summary>
        /// GESTION
        /// </summary>
        private IProcesoRepository _proceso;
        private ISedeSapRepository _sedeSap;
        private IMonedaSapRepository _monedaSap;
        private IAlmacenSapRepository _almacenSap;
        private ITiempoVidaRepository _tiempoVida;
        private IImpuestoSapRepository _impuestoSap;
        private IVehiculoSapRepository _vehiculoSap;
        private IConductorSapRepository _conductorSap;
        private IUnidadMedidaRepository _unidadMedida;
        private ITipoLaminadoRepository _tipoLaminado;
        private ILongitudAnchoRepository _longitudAncho;
        private ITipoCambioSapRepository _tipoCambioSap;
        private IColorImpresionRepository _colorImpresion;
        private IEmpleadoVentaSapRepository _empleadoVentaSap;
        private IGrupoArticuloSapRepository _grupoArticuloSap;
        private ITipoOperacionSapRepository _tipoOperacionSap;
        private ICampoDefinidoUsuarioRepository _camposUsuario;
        private ICondidcionPagoSapRepository _condidcionPagoSap;
        private ISerieNumeracionSapRepository _serieNumeracionSap;
        private ISubGrupoArticuloSapRepository _subGrupoArticuloSap;
        private ISubGrupoArticulo2SapRepository _subGrupoArticulo2Sap;
        private IGrupoSocioNegocioSapRepository _grupoSocioNegocioSap;
        private ISectorSocioNegocioSapRepository _sectorSocioNegocioSap;
        private ITablaDefinidaUsuarioSapRepository _tablaDefinidaUsuarioSap;

        /// <summary>
        /// INVENTARIO
        /// </summary>
        private IArticuloSapRepository _articuloSap;
        private IDocumentoLecturaSapRepository _documentoLecturaSap;
        private ISolicitudTrasladoSapRepository _solicitudTrasladoSap;
        private ITransferenciaStockSapRepository _transferenciaStockSap;

        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        private IDireccionSapRepository _direccionSap;
        private ISocioNegocioSapRepository _socioNegocioSap;
        private IPersonaContactoSapRepository _personaContactoSap;

        /// <summary>
        /// VENTAS
        /// </summary>
        private IEntregaSapRepository _entregaSap;
        private IOrdenVentaSapRepository _ordenVentaSap;
        private IFacturaVentaSapRepository _facturaVentaSap;

        /// <summary>
        /// FACTURACIÓN ELECTRÓNICA
        /// </summary>
        IGuiaElectronicaSapRepository _guiaElectronicaSap;
        IFacturacionElectronicaSapRepositoy _facturacionElectronicaSap;

        /// <summary>
        /// GESTION DE BANCOS
        /// </summary>
        private IPagoRecibidoSapRepository _pagoRecibidoSap;

        /// <summary>
        /// PRODUCCION
        /// </summary>
        private IOrdenFabricacionSapRepository _ordenFabricacionSap;





        public RepositoryWrapper(IConnectionSQL repoContext, IOptions<ParametrosTokenConfig> tokenConfig, IConfiguration configuration, IHttpClientFactory clientFactory, DataContextFil dc, IMapper am)
        {
            _am = am;
            _dc = dc;
            _repoContext = repoContext;
            _tokenConfig = tokenConfig;
            _configuration = configuration;
            _clientFactory = clientFactory;
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
                    _perfil = new PerfilRepository(_repoContext);
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
                    _usuario = new UsuarioRepository(_repoContext, _configuration, _tokenConfig);
                }
                return _usuario;
            }
        }
        public IPersonaRepository Persona
        {
            get
            {
                if (_persona == null)
                {
                    _persona = new PersonaRepository(_repoContext);
                }
                return _persona;
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
        public ISedeRepository Sede
        {
            get
            {
                if (_sede == null)
                {
                    _sede = new SedeRepository(_repoContext);
                }
                return _sede;
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
        public IFormularioRepository Formulario
        {
            get
            {
                if (_formulario == null)
                {
                    _formulario = new FormularioRepository(_repoContext);
                }
                return _formulario;
            }
        }
        public ITipoDocumentoRepository TipoDocumento
        {
            get
            {
                if (_tipoDocumento == null)
                {
                    _tipoDocumento = new TipoDocumentoRepository(_repoContext);
                }
                return _tipoDocumento;
            }
        }
        public ISerieNumeracionRepository SerieNumeracion
        {
            get
            {
                if (_serieNumeracion == null)
                {
                    _serieNumeracion = new SerieNumeracionRepository(_repoContext);
                }
                return _serieNumeracion;
            }
        }

        /// <summary>
        /// INVENTARIO
        /// </summary>
        public ILecturaRepository Lectura
        {
            get
            {
                if (_lectura == null)
                {
                    _lectura = new LecturaRepository(_repoContext);
                }
                return _lectura;
            }
        }
        public IDocumentoLecturaRepository DocumentoLectura
        {
            get
            {
                if (_documentoLectura == null)
                {
                    _documentoLectura = new DocumentoLecturaRepository(_repoContext);
                }
                return _documentoLectura;
            }
        }
        public ISolicitudTrasladoRepository SolicitudTraslado
        {
            get
            {
                if (_solicitudTraslado == null)
                {
                    _solicitudTraslado = new SolicitudTrasladoRepository(_repoContext, _configuration);
                }
                return _solicitudTraslado;
            }
        }
        public ITransferenciaStockRepositoy TransferenciaStock
        {
            get
            {
                if (_transferenciaStock == null)
                {
                    _transferenciaStock = new TransferenciaStockRepositoy(_repoContext, _configuration);
                }
                return _transferenciaStock;
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
        public ISedeSapRepository SedeSap
        {
            get
            {
                if (_sedeSap == null)
                {
                    _sedeSap = new SedeSapRepository(_repoContext, _configuration);
                }
                return _sedeSap;
            }
        }
        public IMonedaSapRepository MonedaSap
        {
            get
            {
                if (_monedaSap == null)
                {
                    _monedaSap = new MonedaSapRepository(_repoContext, _configuration);
                }
                return _monedaSap;
            }
        }
        public IAlmacenSapRepository AlmacenSap
        {
            get
            {
                if (_almacenSap == null)
                {
                    _almacenSap = new AlmacenSapRepository(_repoContext, _configuration);
                }
                return _almacenSap;
            }
        }
        public ITiempoVidaRepository TiempoVida
        {
            get
            {
                if (_tiempoVida == null)
                {
                    _tiempoVida = new TiempoVidaRepository(_repoContext, _dc);
                }
                return _tiempoVida;
            }
        }
        public IImpuestoSapRepository ImpuestoSap
        {
            get
            {
                if (_impuestoSap == null)
                {
                    _impuestoSap = new ImpuestoSapRepository(_repoContext, _dc);
                }
                return _impuestoSap;
            }
        }
        public IVehiculoSapRepository VehiculoSap
        {
            get
            {
                if (_vehiculoSap == null)
                {
                    _vehiculoSap = new VehiculoSapRepository(_repoContext, _configuration);
                }
                return _vehiculoSap;
            }
        }
        public IConductorSapRepository ConductorSap
        {
            get
            {
                if (_conductorSap == null)
                {
                    _conductorSap = new ConductorSapRepository(_repoContext, _configuration);
                }
                return _conductorSap;
            }
        }
        public IUnidadMedidaRepository UnidadMedida
        {
            get
            {
                if (_unidadMedida == null)
                {
                    _unidadMedida = new UnidadMedidaRepository(_repoContext, _dc);
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
                    _tipoLaminado = new TipoLaminadoRepository(_repoContext, _dc);
                }
                return _tipoLaminado;
            }
        }
        public ILongitudAnchoRepository LongitudAncho
        {
            get
            {
                if (_longitudAncho == null)
                {
                    _longitudAncho = new LongitudAnchoRepository(_repoContext, _dc);
                }
                return _longitudAncho;
            }
        }
        public ITipoCambioSapRepository TipoCambioSap
        {
            get
            {
                if (_tipoCambioSap == null)
                {
                    _tipoCambioSap = new TipoCambioSapRepository(_repoContext, _dc);
                }
                return _tipoCambioSap;
            }
        }
        public IColorImpresionRepository ColorImpresion
        {
            get
            {
                if (_colorImpresion == null)
                {
                    _colorImpresion = new ColorImpresionRepository(_repoContext, _dc);
                }
                return _colorImpresion;
            }
        }
        public IEmpleadoVentaSapRepository EmpleadoVentaSap
        {
            get
            {
                if (_empleadoVentaSap == null)
                {
                    _empleadoVentaSap = new EmpleadoVentaSapRepository(_repoContext, _dc);
                }
                return _empleadoVentaSap;
            }
        }
        public IGrupoArticuloSapRepository GrupoArticuloSap
        {
            get
            {
                if (_grupoArticuloSap == null)
                {
                    _grupoArticuloSap = new GrupoArticuloSapRepository(_repoContext, _dc);
                }
                return _grupoArticuloSap;
            }
        }
        public ITipoOperacionSapRepository TipoOperacionSap
        {
            get
            {
                if (_tipoOperacionSap == null)
                {
                    _tipoOperacionSap = new TipoOperacionSapRepository(_repoContext, _dc);
                }
                return _tipoOperacionSap;
            }
        }
        public ICampoDefinidoUsuarioRepository CampoDefinidoUsuario
        {
            get
            {
                if (_camposUsuario == null)
                {
                    _camposUsuario = new CampoDefinidoUsuarioRepository(_repoContext, _dc);
                }
                return _camposUsuario;
            }
        }
        public ICondidcionPagoSapRepository CondidcionPagoSap
        {
            get
            {
                if (_condidcionPagoSap == null)
                {
                    _condidcionPagoSap = new CondidcionPagoSapRepository(_repoContext, _configuration);
                }
                return _condidcionPagoSap;
            }
        }
        public ISerieNumeracionSapRepository SerieNumeracionSap
        {
            get
            {
                if (_serieNumeracionSap == null)
                {
                    _serieNumeracionSap = new SerieNumeracionSapRepository(_repoContext, _configuration);
                }
                return _serieNumeracionSap;
            }
        }
        public ISubGrupoArticuloSapRepository SubGrupoArticuloSap
        {
            get
            {
                if (_subGrupoArticuloSap == null)
                {
                    _subGrupoArticuloSap = new SubGrupoArticuloSapRepository(_repoContext, _dc);
                }
                return _subGrupoArticuloSap;
            }
        }
        public ISubGrupoArticulo2SapRepository SubGrupoArticulo2Sap
        {
            get
            {
                if (_subGrupoArticulo2Sap == null)
                {
                    _subGrupoArticulo2Sap = new SubGrupoArticulo2SapRepository(_repoContext, _dc);
                }
                return _subGrupoArticulo2Sap;
            }
        }
        public IGrupoSocioNegocioSapRepository GrupoSocioNegocioSap
        {
            get
            {
                if (_grupoSocioNegocioSap == null)
                {
                    _grupoSocioNegocioSap = new GrupoSocioNegocioSapRepository(_repoContext, _configuration);
                }
                return _grupoSocioNegocioSap;
            }
        }
        public ISectorSocioNegocioSapRepository SectorSocioNegocioSap
        {
            get
            {
                if (_sectorSocioNegocioSap == null)
                {
                    _sectorSocioNegocioSap = new SectorSocioNegocioSapRepository(_repoContext, _configuration);
                }
                return _sectorSocioNegocioSap;
            }
        }
        public ITablaDefinidaUsuarioSapRepository TablaDefinidaUsuarioSap        {
            get
            {
                if (_tablaDefinidaUsuarioSap == null)
                {
                    _tablaDefinidaUsuarioSap = new TablaDefinidaUsuarioSapRepository(_repoContext, _configuration);
                }
                return _tablaDefinidaUsuarioSap;
            }
        }
        public IProcesoRepository Proceso
        {
            get
            {
                if (_proceso == null)
                {
                    _proceso = new ProcesoRepository(_repoContext, _dc);
                }
                return _proceso;
            }
        }

        /// <summary>
        /// INVENTARIO
        /// </summary>
        public IArticuloSapRepository ArticuloSap
        {
            get
            {
                if (_articuloSap == null)
                {
                    _articuloSap = new ArticuloSapRepository(_repoContext, _configuration);
                }
                return _articuloSap;
            }
        }
        public IDocumentoLecturaSapRepository DocumentoLecturaSap
        {
            get
            {
                if (_documentoLecturaSap == null)
                {
                    _documentoLecturaSap = new DocumentoLecturaSapRepository(_repoContext, _configuration);
                }
                return _documentoLecturaSap;
            }
        }
        public ISolicitudTrasladoSapRepository SolicitudTrasladoSap
        {
            get
            {
                if (_solicitudTrasladoSap == null)
                {
                    _solicitudTrasladoSap = new SolicitudTrasladoSapRepository(_repoContext, _configuration);
                }
                return _solicitudTrasladoSap;
            }
        }
        public ITransferenciaStockSapRepository TransferenciaStockSap
        {
            get
            {
                if (_transferenciaStockSap == null)
                {
                    _transferenciaStockSap = new TransferenciaStockSapRepository(_repoContext, _configuration);
                }
                return _transferenciaStockSap;
            }
        }

        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        public IDireccionSapRepository DireccionSap
        {
            get
            {
                if (_direccionSap == null)
                {
                    _direccionSap = new DireccionSapRepository(_repoContext, _configuration);
                }
                return _direccionSap;
            }
        }
        public ISocioNegocioSapRepository SocioNegocioSap
        {
            get
            {
                if (_socioNegocioSap == null)
                {
                    _socioNegocioSap = new SocioNegocioSapRepository(_repoContext, _configuration);
                }
                return _socioNegocioSap;
            }
        }
        public IPersonaContactoSapRepository PersonaContactoSap
        {
            get
            {
                if (_personaContactoSap == null)
                {
                    _personaContactoSap = new PersonaContactoSapRepository(_repoContext, _configuration);
                }
                return _personaContactoSap;
            }
        }

        /// <summary>
        /// VENTAS
        /// </summary>
        
        public IEntregaSapRepository EntregaSap
        {
            get
            {
                if (_entregaSap == null)
                {
                    _entregaSap = new EntregaSapRepository(_repoContext, _configuration);
                }
                return _entregaSap;
            }
        }
        public IOrdenVentaSapRepository OrdenVentaSap
        {
            get
            {
                if (_ordenVentaSap == null)
                {
                    _ordenVentaSap = new OrdenVentaSapRepository(_repoContext, _configuration);
                }
                return _ordenVentaSap;
            }
        }
        public IFacturaVentaSapRepository FacturaVentaSap
        {
            get
            {
                if (_facturaVentaSap == null)
                {
                    _facturaVentaSap = new FacturaVentaSapRepository(_repoContext, _configuration);
                }
                return _facturaVentaSap;
            }
        }
        public IOSKCRepository OSKC
        {
            get
            {
                if (_OSKC == null)
                {
                    _OSKC = new OSKCRepository(_repoContext, _configuration, _dc, _am);
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
                    _OSKP = new OSKPRepository(_repoContext, _configuration, _dc, _am);
                }
                return _OSKP;
            }
        }

        /// <summary>
        /// FACTURACIÓN ELECTRÓNICA
        /// </summary>
        public IFacturacionElectronicaSapRepositoy FacturacionElectronicaSap
        {
            get
            {
                if (_facturacionElectronicaSap == null)
                {
                    _facturacionElectronicaSap = new FacturacionElectronicaSapRepositoy(_repoContext, _configuration);
                }
                return _facturacionElectronicaSap;
            }
        }
        public IGuiaElectronicaSapRepository GuiaElectronicaSap
        {
            get
            {
                if (_guiaElectronicaSap == null)
                {
                    _guiaElectronicaSap = new GuiaElectronicaSapRepository(_repoContext, _configuration);
                }
                return _guiaElectronicaSap;
            }
        }

        /// <summary>
        /// GESTION DE BANCOS
        /// </summary>
        public IPagoRecibidoSapRepository PagoRecibidoSap
        {
            get
            {
                if (_pagoRecibidoSap == null)
                {
                    _pagoRecibidoSap = new PagoRecibidoRepository(_repoContext, _configuration);
                }
                return _pagoRecibidoSap;
            }
        }

        /// <summary>
        /// PRODUCCION
        /// </summary>
        public IOrdenFabricacionSapRepository OrdenFabricacionSap
        {
            get
            {
                if (_ordenFabricacionSap == null)
                {
                    _ordenFabricacionSap = new OrdenFabricacionSapRepository(_repoContext, _configuration);
                }
                return _ordenFabricacionSap;
            }
        }
    }
}

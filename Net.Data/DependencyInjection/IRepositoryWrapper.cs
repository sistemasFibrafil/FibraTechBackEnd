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
        IPersonaRepository Persona { get; }
        IDataBaseRepository DataBase { get; }
        IAuditoriaRepository Auditoria { get; }
        IOpcionxPerfilRepository OpcionxPerfil { get; }
        IParametroSistemaRepository ParametroSistema { get; }
        IParametroConexionRepository ParametroConexion { get; }

        /// <summary>
        /// GESTION
        /// </summary>
        ISedeRepository Sede { get; }
        IStatusRepository Status { get; }
        ITiempoRepository Tiempo { get; }
        IFormularioRepository Formulario { get; }
        ITipoDocumentoRepository TipoDocumento { get; }
        ISerieNumeracionRepository SerieNumeracion { get; }

        /// <summary>
        /// INVENTARIO
        /// </summary>
        ILecturaRepository Lectura { get; }
        IDocumentoLecturaRepository DocumentoLectura { get; }
        ISolicitudTrasladoRepository SolicitudTraslado { get; }
        ITransferenciaStockRepositoy TransferenciaStock { get; }

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
        IProcesoRepository Proceso { get; }
        ISedeSapRepository SedeSap { get; }
        IMonedaSapRepository MonedaSap { get; }
        ITiempoVidaRepository TiempoVida { get; }
        IAlmacenSapRepository AlmacenSap { get; }
        IImpuestoSapRepository ImpuestoSap { get; }
        IVehiculoSapRepository VehiculoSap { get; }
        IUnidadMedidaRepository UnidadMedida { get; }
        IConductorSapRepository ConductorSap { get; }
        ITipoLaminadoRepository TipoLaminado { get; }
        ILongitudAnchoRepository LongitudAncho { get; }
        ITipoCambioSapRepository TipoCambioSap { get; }
        IColorImpresionRepository ColorImpresion { get; }
        IEmpleadoVentaSapRepository EmpleadoVentaSap { get; }
        IGrupoArticuloSapRepository GrupoArticuloSap { get; }
        ITipoOperacionSapRepository TipoOperacionSap { get; }
        ICondidcionPagoSapRepository CondidcionPagoSap { get; }
        ISerieNumeracionSapRepository SerieNumeracionSap { get; }
        ISubGrupoArticuloSapRepository SubGrupoArticuloSap { get; }
        ISubGrupoArticulo2SapRepository SubGrupoArticulo2Sap { get; }
        IGrupoSocioNegocioSapRepository GrupoSocioNegocioSap { get; }
        ISectorSocioNegocioSapRepository SectorSocioNegocioSap { get; }
        ITablaDefinidaUsuarioSapRepository TablaDefinidaUsuarioSap { get; }
        ICampoDefinidoUsuarioRepository CampoDefinidoUsuario { get; }
        

        /// <summary>
        /// INVENTARIO
        /// </summary>
        IArticuloSapRepository ArticuloSap { get; }
        IDocumentoLecturaSapRepository DocumentoLecturaSap { get; }
        ISolicitudTrasladoSapRepository SolicitudTrasladoSap { get; }
        ITransferenciaStockSapRepository TransferenciaStockSap { get; }

        /// <summary>
        /// SOCIOS DE NEGOCIOS
        /// </summary>
        IDireccionSapRepository DireccionSap { get; }
        ISocioNegocioSapRepository SocioNegocioSap { get; }
        IPersonaContactoSapRepository PersonaContactoSap { get; }

        /// <summary>
        /// VENTAS
        /// </summary>
        IEntregaSapRepository EntregaSap { get; }
        IOrdenVentaSapRepository OrdenVentaSap { get; }
        IFacturaVentaSapRepository FacturaVentaSap { get; }
        IOSKCRepository OSKC { get; }
        IOSKPRepository OSKP { get; }


        /// <summary>
        /// FACTURACIÓN ELECTRÓNICA
        /// </summary>
        IGuiaElectronicaSapRepository GuiaElectronicaSap { get; }
        IFacturacionElectronicaSapRepositoy FacturacionElectronicaSap { get; }

        /// <summary>
        /// GESTION DE BANCOS
        /// </summary>
        IPagoRecibidoSapRepository PagoRecibidoSap { get; }

        /// <summary>
        /// PRODUCCION
        /// </summary>
        IOrdenFabricacionSapRepository OrdenFabricacionSap { get; }
    }
}

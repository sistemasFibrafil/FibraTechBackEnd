using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class PersonaEntity : BaseEntity
    {
        /// <summary>
        /// IdPersona
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdPersona { get; set; }
        /// <summary>
        /// Usuario
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string Usuario { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string Nombre { get; set; }
        /// <summary>
        /// ApellidoPaterno
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string ApellidoPaterno { get; set; }
        /// <summary>
        /// ApellidoMaterno
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string ApellidoMaterno { get; set; }
        /// <summary>
        /// NombreCompleto
        /// </summary>
        public string NombreCompleto { get => ApellidoPaterno + " " + ApellidoMaterno + " " + Nombre; }
        /// <summary>
        /// NroDocumento
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string NroDocumento { get; set; }
        /// <summary>
        /// NroTelefono
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string NroTelefono { get; set; }
        /// <summary>
        /// IdSede
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? CodSede { get; set; }
        /// <summary>
        /// IsNotRestAlmacen
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? IsNotRestAlmacen { get; set; }
        /// FlgActivo
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgActivo { get; set; }
        /// <summary>
        /// DescripcionPerfil
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionPerfil { get; set; }
        /// <summary>
        /// CodCentroCosto
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string CodCentroCosto { get; set; }
        /// <summary>
        /// DesCentroCosto
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DesCentroCosto { get; set; }
        /// <summary>
        /// EntidadUsuario
        /// </summary>
        public UsuarioEntity EntidadUsuario { get; set; }
        /// <summary>
        /// CodCentroCosto
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 3, ActionType.Everything)]
        public string CodCentro { get; set; }
        /// <summary>
        /// DesCentroCosto
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DesCentro { get; set; }
        /// <summary>
        /// Location
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string NomSede { get; set; }
    }
}

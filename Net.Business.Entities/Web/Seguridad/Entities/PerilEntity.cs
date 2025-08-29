using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class PerilEntity : BaseEntity
    {
        /// <summary>
        /// IdPerfil
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdPerfil { get; set; }
        /// <summary>
        /// DescripcionPerfil
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionPerfil { get; set; }
        /// <summary>
        /// codigoTablaVisualizacion
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string CodigoTablaVisualizacion { get; set; }
        /// <summary>
        /// NombreVisualizacion
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 200, ActionType.Everything)]
        public string NombreVisualizacion { get; set; }
        /// <summary>
        /// FlgActivo
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgActivo { get; set; }
    }
}

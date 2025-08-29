using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class OpcionxPerfilEntity : BaseEntity
    {
        /// <summary>
        /// IdPersona
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdOpcionxPerfil { get; set; }
        /// <summary>
        /// IdOpcion
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdOpcion { get; set; }
        /// <summary>
        /// DescripcionOpcion
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionOpcion { get; set; }
        /// <summary>
        /// IdOpcion
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdMenu { get; set; }
        /// <summary>
        /// IdPerfil
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdPerfil { get; set; }
    }
}

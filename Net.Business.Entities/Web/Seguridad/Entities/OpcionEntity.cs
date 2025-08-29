using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class OpcionEntity : BaseEntity
    {
        /// <summary>
        /// IdPersona
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdOpcion { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdMenu { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string Nombre { get; set; }
        /// <summary>
        /// DescripcionOpcion
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionOpcion { get; set; }
        /// <summary>
        /// KeyOpcion
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 50, ActionType.Everything)]
        public string KeyOpcion { get; set; }
    }
}

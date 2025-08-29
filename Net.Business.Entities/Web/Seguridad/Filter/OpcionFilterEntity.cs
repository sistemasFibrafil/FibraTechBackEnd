using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class OpcionFilterEntity
    {
        /// <summary>
        /// IdUsuario
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdUsuario { get; set; }
        /// <summary>
        /// IdMenu
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdMenu { get; set; }
    }
}

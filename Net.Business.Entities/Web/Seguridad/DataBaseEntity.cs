using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class DataBaseEntity
    {
        /// <summary>
        /// IdPerfil
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string IdDataBase { get; set; }
        /// <summary>
        /// DescripcionPerfil
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionDataBase { get; set; }
    }
}

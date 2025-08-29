using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class AuditoriaFilterEntity
    {
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string IdTransaccional { get; set; }
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string Tabla { get; set; }
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string Campo { get; set; }
    }
}

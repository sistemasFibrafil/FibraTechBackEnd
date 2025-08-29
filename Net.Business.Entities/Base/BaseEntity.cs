using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities
{
    public class BaseEntity
    {
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? RegUsuario { get; set; }

        [DBParameter(SqlDbType.NVarChar, 15, ActionType.Everything)]
        public string RegEstacion { get; set; }
    }
}


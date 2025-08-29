using System;
using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class AuditoriaEntity
    {
        [DBParameter(SqlDbType.Decimal, ActionType.Everything, true)]
        public decimal Correlativo { get; set; }
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string IdTransaccional { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int IdUsuario { get; set; }
        [DBParameter(SqlDbType.VarChar, 15, ActionType.Everything)]
        public string Estacion { get; set; }
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string Esquema { get; set; }
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string Tabla { get; set; }
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string Campo { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime FecHora { get; set; }
        [DBParameter(SqlDbType.VarChar, 1, ActionType.Everything)]
        public string Accion { get; set; }
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string ValorAntiguo { get; set; }
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string ValorNuevo { get; set; }
    }
}

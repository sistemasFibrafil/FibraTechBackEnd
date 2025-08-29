using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class ParametroConexionEntity : BaseEntity
    {
        /// <summary>
        /// IdPersona
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int IdParametroConexion { get; set; }
        /// <summary>
        /// AplicacionServidor
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string AplicacionServidor { get; set; }
        /// <summary>
        /// AplicacionBaseDatos
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string AplicacionBaseDatos { get; set; }
        /// <summary>
        /// AplicacionPasswordOriginal
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string AplicacionPasswordOriginal { get; set; }
        /// <summary>
        /// AplicacionUsuario
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string AplicacionUsuario { get; set; }
        /// <summary>
        /// AplicacionUsuario
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string AplicacionPassword { get; set; }
        /// <summary>
        /// SapServidor
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SapServidor { get; set; }
        /// <summary>
        /// SapBaseDatos
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SapBaseDatos { get; set; }
        /// <summary>
        /// SapUsuario
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SapUsuario { get; set; }
        /// <summary>
        /// SapPassword
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string SapPasswordOriginal { get; set; }
        /// <summary>
        /// SapPassword
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string SapPassword { get; set; }
    }
}

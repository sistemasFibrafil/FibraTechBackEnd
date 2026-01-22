using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class ParametroSistemaEntity : BaseEntity
    {
        /// <summary>
        /// IdParametrosSistema
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int IdParametrosSistema { get; set; }
        /// <summary>
        /// IdTipoAutenticacion
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string TipoAutenticacion { get; set; }
        /// <summary>
        /// FlgDimensionSAP
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgDimensionSAP { get; set; }
        /// <summary>
        /// IdDimensionSAP
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdDimensionSAP { get; set; }
        /// <summary>
        /// FlgGoogleDrive
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgGoogleDrive { get; set; }
        /// <summary>
        /// FlgDobleAutenticacion
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgDobleAutenticacion { get; set; }
        /// <summary>
        /// SendEmail
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SendEmail { get; set; }
        /// <summary>
        /// SendEmailPassword
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string SendEmailPassword { get; set; }
        /// <summary>
        /// SendEmailPassword
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 30, ActionType.Everything)]
        public string SendEmailPasswordOrigen { get; set; }
        /// <summary>
        /// SendEmailPort
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? SendEmailPort { get; set; }
        /// <summary>
        /// SendEmailEnabledSSL
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public bool? SendEmailEnabledSSL { get; set; }
        /// <summary>
        /// SendEmailHost
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SendEmailHost { get; set; }

        /// <summary>
        /// SendEmailFinanza
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SendEmailFinanza { get; set; }
        /// <summary>
        /// SendEmailFinanzaPassword
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string SendEmailFinanzaPassword { get; set; }
        /// <summary>
        /// SendEmailFinanzaPassword
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 30, ActionType.Everything)]
        public string SendEmailFinanzaPasswordOrigen { get; set; }
        /// <summary>
        /// SendEmailFinanzaPort
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? SendEmailFinanzaPort { get; set; }
        /// <summary>
        /// SendEmailFinanzaEnabledSSL
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public bool? SendEmailFinanzaEnabledSSL { get; set; }
        /// <summary>
        /// SendEmailFinanzaHost
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string SendEmailFinanzaHost { get; set; }

        /// <summary>
        /// AsuntoFinanza
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string AsuntoFinanza { get; set; }
        /// <summary>
        /// CuerpoFinanza
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string CuerpoFinanza { get; set; }
        /// <summary>
        /// DiasPorVencerFinanza
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? DiasPorVencerFinanza { get; set; }
        /// <summary>
        /// HoraEnvioFinanza
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 8, ActionType.Everything)]
        public string HoraEnvioFinanza { get; set; }

        /// <summary>
        /// EmailGoogleDrive
        /// </summary>
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string EmailGoogleDrive { get; set; }
        /// <summary>
        /// EmailPassword
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string EmailPassword { get; set; }
    }
}

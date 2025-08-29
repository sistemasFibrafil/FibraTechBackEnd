using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class ParametroSistemaUpdateRequestDto : BaseEntity
    {
        public int IdParametrosSistema { get; set; }
        public string TipoAutenticacion { get; set; }
        public bool FlgDimensionSAP { get; set; }
        public int IdDimensionSAP { get; set; }
        public bool FlgGoogleDrive { get; set; }
        public bool FlgDobleAutenticacion { get; set; }
        public string SendEmail { get; set; }
        public string SendEmailPasswordOrigen { get; set; }
        public int SendEmailPort { get; set; }
        public bool SendEmailEnabledSSL { get; set; }
        public string SendEmailHost { get; set; }

        public string SendEmailFinanza { get; set; }
        public string SendEmailFinanzaPasswordOrigen { get; set; }
        public int SendEmailFinanzaPort { get; set; }
        public bool SendEmailFinanzaEnabledSSL { get; set; }
        public string SendEmailFinanzaHost { get; set; }

        public string AsuntoFinanza { get; set; }
        public string CuerpoFinanza { get; set; }
        public int DiasPorVencerFinanza { get; set; }
        public string HoraEnvioFinanza { get; set; }

        public string EmailGoogleDrive { get; set; }
        public string EmailPassword { get; set; }

        public ParametroSistemaEntity RetornaParametroSistema()
        {
            return new ParametroSistemaEntity
            {
                IdParametrosSistema = IdParametrosSistema,
                TipoAutenticacion = TipoAutenticacion,
                FlgDimensionSAP = FlgDimensionSAP,
                IdDimensionSAP = IdDimensionSAP,
                FlgGoogleDrive = FlgGoogleDrive,
                FlgDobleAutenticacion = FlgDobleAutenticacion,
                SendEmail = SendEmail,
                SendEmailPasswordOrigen = SendEmailPasswordOrigen,
                SendEmailPort = SendEmailPort,
                SendEmailEnabledSSL = SendEmailEnabledSSL,
                SendEmailHost = SendEmailHost,

                SendEmailFinanza = SendEmailFinanza,
                SendEmailFinanzaPasswordOrigen = SendEmailFinanzaPasswordOrigen,
                SendEmailFinanzaPort = SendEmailFinanzaPort,
                SendEmailFinanzaEnabledSSL = SendEmailFinanzaEnabledSSL,
                SendEmailFinanzaHost = SendEmailFinanzaHost,

                AsuntoFinanza = AsuntoFinanza,
                CuerpoFinanza = CuerpoFinanza,
                DiasPorVencerFinanza = DiasPorVencerFinanza,
                HoraEnvioFinanza = HoraEnvioFinanza,

                EmailGoogleDrive = EmailGoogleDrive,
                EmailPassword = EmailPassword,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}

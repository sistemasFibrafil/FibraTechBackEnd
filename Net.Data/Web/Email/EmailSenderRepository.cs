using System.Net;
using Net.Connection;
using System.Net.Mail;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class EmailSenderRepository : RepositoryBase<EmailSenderOptionsEntity>, IEmailSenderRepository
    {
        const string DB_ESQUEMA = "DBO.";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetParametroSistemaPorId";
        private SmtpClient Cliente { get; }
        private ParametroSistemaEntity Options { get; }

        public EmailSenderRepository(IConnectionSQL context)
            : base(context)
        {

            Options = context.ExecuteSqlViewId<ParametroSistemaEntity>(SP_GET_ID, new ParametroSistemaEntity { IdParametrosSistema = 0 });

            Options.SendEmailPasswordOrigen = EncriptaHelper.DecryptStringAES(Options.SendEmailPassword);

            Cliente = new SmtpClient()
            {
                EnableSsl = (bool)Options.SendEmailEnabledSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Options.SendEmail, Options.SendEmailPasswordOrigen),
                Host = Options.SendEmailHost,
                Port = (int)Options.SendEmailPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };
        }

        public List<string> ListarEmail(string email)
        {

            List<string> lista = new List<string>();

            var posicion = email.IndexOf(";");

            var posiInicio = 0;

            if (posicion == -1)
            {
                lista.Add(email);
            }

            while (posicion != -1)
            {

                var data = email.Substring(posiInicio, posicion);
                lista.Add(data);
                email = email.Substring(posicion + 1);
                posicion = email.IndexOf(';');

                if (posicion == -1)
                {
                    lista.Add(email);
                }
            }

            return lista;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var listEmail = ListarEmail(email);
            var emailTo = string.Empty;
            if (listEmail.Count > 0)
            {
                emailTo = listEmail[0];
                listEmail.RemoveAt(0);
            }

            var correo = new MailMessage(from: Options.SendEmail, to: emailTo, subject: subject, body: message);
            foreach (var item in listEmail)
            {
                correo.To.Add(item);
            }
            correo.IsBodyHtml = true;
            return Cliente.SendMailAsync(correo);
        }
    }
}

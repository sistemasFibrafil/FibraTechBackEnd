using System.Data;
using Net.Connection.Attributes;

namespace Net.Business.Entities.Web
{
    public class UsuarioUpdatePasswordEntity
    {
        public int IdUsuario { get; set; }
        public string Clave { get; set; }
    }
}

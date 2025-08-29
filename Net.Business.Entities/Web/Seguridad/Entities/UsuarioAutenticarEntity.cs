namespace Net.Business.Entities.Web
{
    public class UsuarioAutenticarEntity
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public bool FlgDobleAutenticacion { get; set; }

    }
}

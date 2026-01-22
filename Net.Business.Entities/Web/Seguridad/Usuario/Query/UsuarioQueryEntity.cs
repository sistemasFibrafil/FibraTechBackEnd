namespace Net.Business.Entities.Web
{
    public class UsuarioQueryEntity
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string NroDocumento { get; set; }
        public string DescripcionPerfil { get; set; }
        public bool? Activo { get; set; }
    }
}

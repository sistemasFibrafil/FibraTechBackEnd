using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PersonaUpdateRequestDto : BaseEntity
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NroDocumento { get; set; }
        public string NroTelefono { get; set; }
        public int? CodSede { get; set; }
        public bool? IsNotRestAlmacen { get; set; }
        public bool FlgActivo { get; set; }
        public string CodCentroCosto { get; set; }
        public string CodCentro { get; set; }
        public UsuarioEntity EntidadUsuario { get; set; }
        public PersonaEntity RetornaPersona()
        {
            return new PersonaEntity
            {
                IdPersona = IdPersona,
                Nombre = Nombre,
                ApellidoPaterno = ApellidoPaterno,
                ApellidoMaterno = ApellidoMaterno,
                NroDocumento = NroDocumento,
                NroTelefono = NroTelefono,
                CodSede = CodSede,
                IsNotRestAlmacen = IsNotRestAlmacen,
                FlgActivo = FlgActivo,
                EntidadUsuario = EntidadUsuario,
                CodCentroCosto = CodCentroCosto,
                CodCentro = CodCentro,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}

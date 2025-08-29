using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class SolicitudTrasladoCloseRequestDto
    {
        public int Id { get; set; }
        public int DocEntry { get; set; } = 0;
        public int? IdUsuarioClose { get; set; } = null;

        public SolicitudTrasladoEntity ReturnValue()
        {
            return new SolicitudTrasladoEntity()
            {
                Id = Id,
                DocEntry = DocEntry,
                IdUsuarioClose = IdUsuarioClose,
            };
        }
    }
}

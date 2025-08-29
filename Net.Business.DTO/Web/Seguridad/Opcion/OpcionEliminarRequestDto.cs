using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OpcionEliminarRequestDto : BaseEntity
    {
        public int IdOpcion { get; set; }

        public OpcionEntity RetornarOpcion()
        {
            return new OpcionEntity
            {
                IdOpcion = IdOpcion,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}

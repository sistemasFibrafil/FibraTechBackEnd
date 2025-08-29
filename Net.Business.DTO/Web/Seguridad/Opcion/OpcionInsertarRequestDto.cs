using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OpcionInsertarRequestDto : BaseEntity
    {
        public int IdOpcion { get; set; }
        public int IdMenu { get; set; }
        public string DescripcionOpcion { get; set; }
        public string KeyOpcion { get; set; }

        public OpcionEntity RetornarOpcion()
        {
            return new OpcionEntity
            {
                IdOpcion = IdOpcion,
                IdMenu = IdMenu,
                DescripcionOpcion = DescripcionOpcion,
                KeyOpcion = KeyOpcion,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}

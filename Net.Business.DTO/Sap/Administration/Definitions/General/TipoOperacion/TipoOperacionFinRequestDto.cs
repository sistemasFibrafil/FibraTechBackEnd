using Net.Business.Entities.Sap;
namespace Net.Business.DTO
{
    public class TipoOperacionFinRequestDto
    {
        public string TipoOperacion { get; set; }

        public TipoOperacionFilterEntity ReturnValue()
        {
            return new TipoOperacionFilterEntity
            {
                TipoOperacion = TipoOperacion
            };
        }
    }
}

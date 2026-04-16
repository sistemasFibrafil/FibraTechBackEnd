using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OperationTypeFinRequestDto
    {
        public string? TipoOperacion { get; set; }

        public TipoOperacionFilterEntity ReturnValue()
        {
            return new TipoOperacionFilterEntity
            {
                TipoOperacion = TipoOperacion
            };
        }
    }
}

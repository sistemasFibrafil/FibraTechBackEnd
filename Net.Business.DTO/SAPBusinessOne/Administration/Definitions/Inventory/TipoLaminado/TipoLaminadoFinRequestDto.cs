using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TipoLaminadoFinRequestDto
    {
        public string Name { get; set; }

        public TipoLaminadoEntity ReturnValue()
        {
            return new TipoLaminadoEntity
            {
                Name = Name
            };
        }
    }
}

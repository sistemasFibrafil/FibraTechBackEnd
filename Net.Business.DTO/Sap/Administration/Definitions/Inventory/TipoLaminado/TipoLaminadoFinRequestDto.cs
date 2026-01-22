using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
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

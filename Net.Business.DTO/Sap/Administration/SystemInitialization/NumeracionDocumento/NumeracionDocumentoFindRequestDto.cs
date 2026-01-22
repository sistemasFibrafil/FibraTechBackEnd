using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class NumeracionDocumentoFindRequestDto
    {
        public string ObjectCode { get; set; }
        public NumeracionDocumentoEntity ReturnValue()
        {
            return new NumeracionDocumentoEntity
            {
                ObjectCode = ObjectCode
            };
        }
    }
}

using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class NumeracionDocumentoSunatFindRequestDto
    {
        public string U_BPP_NDTD { get; set; }
        public string U_BPP_NDSD { get; set; }

        public NumeracionDocumentoSunatEntity ReturnValue()
        {
            return new NumeracionDocumentoSunatEntity
            {
                U_BPP_NDTD = U_BPP_NDTD,
                U_BPP_NDSD = U_BPP_NDSD
            };
        }
    }
}

using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class NumeracionDocumentoSunatFilterRequestDto
    {
        public string U_BPP_NDTD { get; set; }
        public string U_BPP_NDCD { get; set; }
        public string U_FIB_TDED { get; set; }
        public string U_FIB_TDTD { get; set; }
        public int U_FIB_SEDE { get; set; }

        public NumeracionDocumentoSunatEntity ReturnValue()
        {
            return new NumeracionDocumentoSunatEntity
            {
                U_BPP_NDTD = U_BPP_NDTD,
                U_BPP_NDCD = U_BPP_NDCD,
                U_FIB_TDED = U_FIB_TDED,
                U_FIB_TDTD = U_FIB_TDTD,
                U_FIB_SEDE = U_FIB_SEDE,
            };
        }
    }
}

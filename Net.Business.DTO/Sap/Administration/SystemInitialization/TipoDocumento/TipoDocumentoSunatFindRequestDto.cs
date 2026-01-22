using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TipoDocumentoSunatFindRequestDto
    {
        public string U_FIB_TDTD { get; set; }

        public TipoDocumentoSunatEntity ReturnValue()
        {
            return new TipoDocumentoSunatEntity
            {
                U_FIB_TDTD = U_FIB_TDTD
            };
        }
    }
}

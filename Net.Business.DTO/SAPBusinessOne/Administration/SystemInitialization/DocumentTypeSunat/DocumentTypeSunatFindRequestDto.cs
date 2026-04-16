using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class DocumentTypeSunatFindRequestDto
    {
        public string? U_FIB_ENTR { get; set; }
        public string? U_FIB_FAVE { get; set; }
        public string? U_FIB_TRAN { get; set; }

        public DocumentTypeSunatEntity ReturnValue()
        {
            return new DocumentTypeSunatEntity
            {
                U_FIB_ENTR = U_FIB_ENTR,
                U_FIB_FAVE = U_FIB_FAVE,
                U_FIB_TRAN = U_FIB_TRAN
            };
        }
    }
}

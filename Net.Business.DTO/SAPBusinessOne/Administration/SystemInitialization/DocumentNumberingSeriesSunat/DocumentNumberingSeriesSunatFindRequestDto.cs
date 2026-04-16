using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class DocumentNumberingSeriesSunatFindRequestDto
    {
        public string? U_BPP_NDTD { get; set; }
        public string? U_BPP_NDSD { get; set; }

        public DocumentNumberingSeriesSunatEntity ReturnValue()
        {
            return new DocumentNumberingSeriesSunatEntity
            {
                U_BPP_NDTD = U_BPP_NDTD,
                U_BPP_NDSD = U_BPP_NDSD
            };
        }
    }
}

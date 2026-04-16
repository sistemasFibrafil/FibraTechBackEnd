using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class DocumentNumberingSeriesSunatFilterRequestDto
    {
        public int IdUsuario { get; set; }
        public string? U_BPP_NDTD { get; set; }
        public string? U_BPP_NDCD { get; set; }
        public string? U_SalesInvoices { get; set; }
        public string? U_Delivery { get; set; }
        public string? U_Transfer { get; set; }

        public DocumentNumberingSeriesSunatFindEntity ReturnValue()
        {
            return new DocumentNumberingSeriesSunatFindEntity
            {
                IdUsuario = IdUsuario,
                U_BPP_NDTD = U_BPP_NDTD,
                U_BPP_NDCD = U_BPP_NDCD,
                U_SalesInvoices = U_SalesInvoices,
                U_Delivery = U_Delivery,
                U_Transfer = U_Transfer,
            };
        }
    }
}

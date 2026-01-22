using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class ItemsByDocumentsFindRequestDto
    {
        public string ItemCode { get; set; }
        public string CardCode { get; set; }
        public string Currency { get; set; }
        public int SlpCode { get; set; }
        public string CodTipoOperacion { get; set; }

        public ItemsByDocumentsFindEntity ReturnValue()
        {
            return new ItemsByDocumentsFindEntity
            {
                ItemCode = ItemCode,
                CardCode = CardCode,
                Currency = Currency,
                SlpCode = SlpCode,
                CodTipoOperacion = CodTipoOperacion
            };
        }
    }
}

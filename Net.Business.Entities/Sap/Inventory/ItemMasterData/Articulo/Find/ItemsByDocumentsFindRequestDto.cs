namespace Net.Business.Entities.Sap
{
    public class ItemsByDocumentsFindRequestDto
    {
        public string ItemCode { get; set; }
        public string TipoOperacion { get; set; }

        public ItemsByDocumentsFindEntity ReturnValue()
        {
            return new ItemsByDocumentsFindEntity
            {
                ItemCode = this.ItemCode,
                CodTipoOperacion = this.TipoOperacion
            };
        }
    }
}

namespace Net.Business.Entities.SAPBusinessOne
{
    public class ItemsByDocumentsFindRequestDto
    {
        public string ItemCode { get; set; }
        public string TipoOperacion { get; set; }

        public ItemsFindByListCodeEntity ReturnValue()
        {
            return new ItemsFindByListCodeEntity
            {
                ItemCode = ItemCode,
                OperationTypeCode = TipoOperacion
            };
        }
    }
}

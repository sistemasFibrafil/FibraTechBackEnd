using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsFindByCodeRequestDto
    {
        public string? ItemCode { get; set; }
        public string? CardCode { get; set; }
        public string? Currency { get; set; }
        public string? OperationTypeCode { get; set; }
        public string? WarehouseType { get; set; }

        public ItemsFindByCodeEntity ReturnValue()
        {
            return new ItemsFindByCodeEntity
            {
                ItemCode = ItemCode,
                CardCode = CardCode,
                Currency = Currency,
                OperationTypeCode = OperationTypeCode,
                WarehouseType = WarehouseType
            };
        }
    }
}

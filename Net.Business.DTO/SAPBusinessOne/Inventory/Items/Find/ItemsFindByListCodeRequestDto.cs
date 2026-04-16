using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsFindByListCodeRequestDto
    {
        public string? ItemCode { get; set; }
        public string? CardCode { get; set; }
        public string? Currency { get; set; }
        public string? OperationTypeCode { get; set; }
        public string? WarehouseProduction { get; set; }
        public string? WarehouseLogistics { get; set; }

        public ItemsFindByListCodeEntity ReturnValue()
        {
            return new ItemsFindByListCodeEntity
            {
                ItemCode = ItemCode,
                CardCode = CardCode,
                Currency = Currency,
                OperationTypeCode = OperationTypeCode,
                WarehouseProduction = WarehouseProduction,
                WarehouseLogistics = WarehouseLogistics
            };
        }
    }
}

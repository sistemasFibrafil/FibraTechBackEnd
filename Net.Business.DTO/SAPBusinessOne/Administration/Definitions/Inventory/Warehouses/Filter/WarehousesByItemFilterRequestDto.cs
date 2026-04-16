using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class WarehousesByItemFilterRequestDto
    {
        public string? ItemCode { get; set; }
        public string? Inactive { get; set; }
        public string? Warehouse { get; set; }

        public WarehousesByItemFilterEntity ReturnValue()
        {
            return new WarehousesByItemFilterEntity
            {
                ItemCode = ItemCode,
                Inactive = Inactive,
                Warehouse = Warehouse
            };
        }
    }
}

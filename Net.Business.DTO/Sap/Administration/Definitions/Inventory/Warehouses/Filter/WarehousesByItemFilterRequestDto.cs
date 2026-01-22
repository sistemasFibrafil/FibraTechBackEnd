using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class WarehousesByItemFilterRequestDto
    {
        public string ItemCode { get; set; }
        public string Inactive { get; set; }
        public string Warehouse { get; set; }

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

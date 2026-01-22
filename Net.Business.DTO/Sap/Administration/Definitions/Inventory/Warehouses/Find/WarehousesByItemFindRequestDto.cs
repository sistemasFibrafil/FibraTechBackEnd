using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class WarehousesByItemFindRequestDto
    {
        public string ItemCode { get; set; }
        public string Inactive { get; set; }
        public string WhsCode { get; set; }

        public WarehousesByItemFindEntity ReturnValue()
        {
            return new WarehousesByItemFindEntity
            {
                ItemCode = ItemCode,
                Inactive = Inactive,
                WhsCode = WhsCode
            };
        }
    }
}

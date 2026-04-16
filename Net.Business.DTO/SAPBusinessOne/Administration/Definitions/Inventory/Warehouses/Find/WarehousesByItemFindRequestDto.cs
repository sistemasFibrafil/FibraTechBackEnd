using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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

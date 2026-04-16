using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsFilterRequestDto
    {
        public string? Item { get; set; }
        public string? InvntItem { get; set; }
        public string? SellItem { get; set; }
        public string? PrchseItem { get; set; }

        public ItemsFilterEntity ReturnValue()
        {
            return new ItemsFilterEntity
            {
                Item = Item,
                InvntItem = InvntItem,
                SellItem = SellItem,
                PrchseItem = PrchseItem
            };
        }
    }
}

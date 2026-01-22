using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class ItemsFilterRequestDto
    {
        public string Item { get; set; }
        public string InvntItem { get; set; }
        public string SellItem { get; set; }
        public string PrchseItem { get; set; }

        public ItemsFilterEntity ReturnValue()
        {
            return new ItemsFilterEntity
            {
                Item = this.Item,
                InvntItem = this.InvntItem,
                SellItem = this.SellItem,
                PrchseItem = this.PrchseItem
            };
        }
    }
}

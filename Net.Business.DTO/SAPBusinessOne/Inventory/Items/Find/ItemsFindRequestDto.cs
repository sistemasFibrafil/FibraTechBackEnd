using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsFindRequestDto
    {
        public string ItemCode { get; set; }
        public ItemsEntity ReturnValue()
        {
            return new ItemsEntity
            {
                ItemCode = ItemCode
            };
        }
    }
}

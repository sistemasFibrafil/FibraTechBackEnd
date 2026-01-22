using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class ItemsFindRequestDto
    {
        public string ItemCode { get; set; }
        public ItemsEntity ReturnValue()
        {
            return new ItemsEntity
            {
                ItemCode = this.ItemCode
            };
        }
    }
}

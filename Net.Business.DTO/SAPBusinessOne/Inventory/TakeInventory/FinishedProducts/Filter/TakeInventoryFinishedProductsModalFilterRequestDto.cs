using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventoryFinishedProductsModalFilterRequestDto
    {
        public int DocEntry { get; set; }
        public string? CodeBar { get; set; }

        public TakeInventoryFinishedProductsModalFilterEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsModalFilterEntity
            {
                DocEntry = DocEntry,
                CodeBar = CodeBar,
            };
        }
    }
}

using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventoryFinishedProductsModalFilterRequestDto
    {
        public int DocEntry { get; set; }
        public string CodeBar { get; set; }

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

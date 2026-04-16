using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventoryFinishedProductsToCopyFindRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Usuario { get; set; }
        public string? WhsCode { get; set; }
        public string? Item { get; set; }
        public string? ItemCode { get; set; }

        public TakeInventoryFinishedProductsToCopyFindEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsToCopyFindEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Usuario = Usuario,
                WhsCode = WhsCode,
                Item = Item,
                ItemCode = ItemCode
            };
        }
    }
}

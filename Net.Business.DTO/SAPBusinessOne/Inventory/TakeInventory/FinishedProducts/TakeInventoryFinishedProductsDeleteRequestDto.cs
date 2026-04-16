using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventoryFinishedProductsDeleteRequestDto
    {
        public int DocEntry { get; set; }
        public string IsDelete { get; set; }
        public int UsrDelete { get; set; }
        public DateTime DeleteDate { get; set; }
        public short DeleteTime { get; set; }

        public TakeInventoryFinishedProductsDeleteEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsDeleteEntity
            {
                DocEntry = DocEntry,
                IsDelete = IsDelete,
                UsrDelete = UsrDelete,
                DeleteDate = DeleteDate,
                DeleteTime = DeleteTime,
            };
        }
    }
}

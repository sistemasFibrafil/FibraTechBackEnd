using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventoryFinishedProductsDeleteRequestDto
    {
        public int DocEntry { get; set; }
        public string IsDelete { get; set; }
        public int UsrDelete { get; set; }
        public DateTime DeleteDate { get; set; }
        public Int16 DeleteTime { get; set; }

        public TakeInventoryFinishedProductsDeleteEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsDeleteEntity
            {
                DocEntry = this.DocEntry,
                IsDelete = this.IsDelete,
                UsrDelete = this.UsrDelete,
                DeleteDate = this.DeleteDate,
                DeleteTime = this.DeleteTime,
            };
        }
    }
}

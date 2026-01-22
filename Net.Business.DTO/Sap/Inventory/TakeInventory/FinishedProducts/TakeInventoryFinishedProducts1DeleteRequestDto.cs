using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventoryFinishedProducts1DeleteRequestDto
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string IsDelete { get; set; }
        public int UsrDelete { get; set; }
        public DateTime DeleteDate { get; set; }
        public Int16 DeleteTime { get; set; }

        public TakeInventoryFinishedProducts1DeleteEntity ReturnValue()
        {
            return new TakeInventoryFinishedProducts1DeleteEntity
            {
                DocEntry = this.DocEntry,
                LineId = this.LineId,
                IsDelete = this.IsDelete,
                UsrDelete = this.UsrDelete,
                DeleteDate = this.DeleteDate,
                DeleteTime = this.DeleteTime,
            };
        }
    }
}

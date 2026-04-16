using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventoryFinishedProducts1DeleteRequestDto
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string IsDelete { get; set; }
        public int UsrDelete { get; set; }
        public DateTime DeleteDate { get; set; }
        public short DeleteTime { get; set; }

        public TakeInventoryFinishedProducts1DeleteEntity ReturnValue()
        {
            return new TakeInventoryFinishedProducts1DeleteEntity
            {
                DocEntry = DocEntry,
                LineId = LineId,
                IsDelete = IsDelete,
                UsrDelete = UsrDelete,
                DeleteDate = DeleteDate,
                DeleteTime = DeleteTime,
            };
        }
    }
}

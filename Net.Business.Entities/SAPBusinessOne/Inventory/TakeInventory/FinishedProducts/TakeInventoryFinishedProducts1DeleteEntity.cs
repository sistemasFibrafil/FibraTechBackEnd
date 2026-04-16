using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class TakeInventoryFinishedProducts1DeleteEntity
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string IsDelete { get; set; }
        public int UsrDelete { get; set; }
        public DateTime DeleteDate { get; set; }
        public short DeleteTime { get; set; }
    }
}

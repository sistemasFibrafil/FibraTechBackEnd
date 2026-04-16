using System;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class PickingFilterEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ObjType { get; set; }
        public string? Status { get; set; }
        public string? SearchText { get; set; }
    }
}

using System;
namespace Net.Business.Entities.Sap
{
    public class TakeInventoryFinishedProductsDeleteEntity
    {
        public int DocEntry { get; set; }
        public string IsDelete { get; set; }
        public int UsrDelete { get; set; }
        public DateTime DeleteDate { get; set; }
        public Int16 DeleteTime { get; set; }
    }
}

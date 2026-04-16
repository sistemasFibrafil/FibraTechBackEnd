using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class TakeInventorySparePartsDeleteEntity
    {
        public int DocEntry { get; set; }
        public string U_WhsCode { get; set; }
        public string U_IsDelete { get; set; }
        public int U_UsrDelete { get; set; }
        public DateTime U_DeleteDate { get; set; }
        public DateTime U_DeleteTime { get; set; }
    }
}

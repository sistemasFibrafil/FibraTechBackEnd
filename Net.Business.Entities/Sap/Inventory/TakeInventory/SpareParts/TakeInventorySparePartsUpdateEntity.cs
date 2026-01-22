using System;
namespace Net.Business.Entities.Sap
{
    public class TakeInventorySparePartsUpdateEntity
    {
        public int DocEntry { get; set; }
        public string U_WhsCode { get; set; }
        public decimal U_OnHandPhy { get; set; }
        public decimal U_Difference { get; set; }
        public int U_UsrUpdate { get; set; }
        public DateTime U_UpdateDate { get; set; }
        public DateTime U_UpdateTime { get; set; }
    }
}

using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class TakeInventorySparePartsQueryEntity
    {
        public int DocEntry { get; set; }
        public string U_ItemCode { get; set; }
        public string U_Dscription { get; set; }
        public string U_WhsCode { get; set; }
        public string U_UnitMsr { get; set; }
        public decimal U_OnHandPhy { get; set; }
        public decimal U_OnHandSys { get; set; }
        public decimal U_Difference { get; set; }
        public int U_UsrCreate { get; set; }
        public string U_UsrNameCreate { get; set; }
        public DateTime U_CreateDate { get; set; }
        public short U_CreateTime { get; set; }
    }
}

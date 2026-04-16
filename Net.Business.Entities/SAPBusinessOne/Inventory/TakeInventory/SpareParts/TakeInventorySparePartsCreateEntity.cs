using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class TakeInventorySparePartsCreateEntity
    {
        public string U_WhsCode { get; set; }
        public string U_CodeBar { get; set; }
        public int U_UsrCreate { get; set; }
        public DateTime U_CreateDate { get; set; }
        public DateTime U_CreateTime { get; set; }
    }
}

using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class PickingCreateEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
    }

    public class Picking1CreateEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public string U_FIB_LinStPkg { get; set; } = null;
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public string FromWhsCod { get; set; }
        public string UnitMsr { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
    }
}

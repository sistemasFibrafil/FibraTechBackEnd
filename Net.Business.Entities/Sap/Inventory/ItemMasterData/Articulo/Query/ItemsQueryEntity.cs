using System;
namespace Net.Business.Entities.Sap
{
    public class ItemsQueryEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Int16 ItmsGrpCod { get; set; }
        public string InvntItem { get; set; }
        public string SellItem { get; set; }
        public string PrchseItem { get; set; }
        public string WTLiable { get; set; }
        public string VatLiable { get; set; }
        public string IndirctTax { get; set; }
        public string frozenFor { get; set; }
        public string SalUnitMsr { get; set; }
        public string BuyUnitMsr { get; set; }
        public string InvntryUom { get; set; }
        public decimal SalPackUn { get; set; }
        public string DfltWH { get; set; }

        public string AcctCode { get; set; }
        public string FormatCode { get; set; }
        public string AcctName { get; set; }

        public decimal? OnHand { get; set; } = 0;
        public string Currency { get; set; }
        public decimal PriceBefDi { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public decimal Quantity { get; set; } = 0;
        public string TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal LineTotal { get; set; }
        public decimal VatSum { get; set; }
        public string U_FIB_ItemCode { get; set; }
        public string U_FIB_ItemName { get; set; }
        public string U_BPP_TIPEXIST { get; set; }
        public string U_BPP_TIPUNMED { get; set; }
        public string U_S_PartAranc1 { get; set; }
        public string U_S_PartAranc2 { get; set; }
        public string U_FIB_ECU { get; set; }
        public string U_S_CCosto { get; set; }
        public decimal? U_FIB_PESO { get; set; } = 0;
        public string U_FIB_SGRUP { get; set; }
        public string U_FIB_SGRUPO2 { get; set; }
        public string U_FIB_LINNEG { get; set; }



        // Tipo de operación
        public string U_tipoOpT12 { get; set; }
        public string U_tipoOpT12Nam { get; set; }
    }
}

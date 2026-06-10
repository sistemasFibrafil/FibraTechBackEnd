using System;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Entities;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class PurchaseRequestLinesEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string? LineStatus { get; set; }
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? LineVendor { get; set; }
        public DateTime PQTReqDate { get; set; }
        public string? AcctCode { get; set; }
        public string? OcrCode { get; set; }
        public string? WhsCode { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public string? U_FF_TIP_COM { get; set; }
        public string? UnitMsr { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? OpenQty { get; set; }


        // 🔗 N → 1 (PRQ1 → OITM)
        public ItemsEntity Item { get; set; } = null!;


        // 🔗 N → 1 (PRQ1 → ChartOfAccounts)
        public ChartOfAccountsEntity? ChartOfAccounts { get; set; } = null;


        // 🔗 N → 1 (PRQ1 → TipoOperacion)
        public OperationsTypesEntity? OperationType { get; set; } = null;
    }
}

using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class OSKCViewEntity
    {
        public string Code { get; set; }
        public string U_Number { get; set; } = null;
        public int U_SlpCode { get; set; }
        public string U_SlpName { get; set; } = null;
        public string U_Status { get; set; } = null;
        public string U_StatusName { get; set; } = null;
        public DateTime? U_DocDate { get; set; } = null;
        public string U_ItemCodeBase { get; set; } = null;
        public string U_ItemNameBase { get; set; } = null;
        public int U_ItmsGrpCod { get; set; }
        public string U_ItmsGrpNam { get; set; } = null;
        public string U_ItmsSGrpCod { get; set; } = null;
        public string U_ItmsSGrpNam { get; set; } = null;
        public string U_ItemName { get; set; } = null;
        public string U_CardCode { get; set; } = null;
        public string U_CardName { get; set; } = null;
        public decimal U_Quantity { get; set; }
        public string U_UnitMsrCode { get; set; } = null;
        public string U_UnitMsrName { get; set; }
        public decimal U_Wide { get; set; }
        public short U_UnitCode { get; set; }
        public string U_UnitName { get; set; }
        public decimal U_Long { get; set; }
        public decimal U_GrMtSq { get; set; }
        public decimal U_ItemWeight { get; set; }
        public string U_ColorCode { get; set; } = null;
        public string U_ColorName { get; set; } = null;
        public string U_Laminate { get; set; } = null;
        public string U_LaminateName { get; set; } = null;
        public string U_LamTypCode { get; set; } = null;
        public string U_LamTypName { get; set; } = null;
        public string U_Linner { get; set; } = null;
        public string U_LinnerName { get; set; } = null;
        public decimal U_LinnWeight { get; set; }
        public string U_Print { get; set; } = null;
        public string U_PrintName { get; set; } = null;
        public string U_PrintColCode { get; set; } = null;
        public string U_PrintColName { get; set; } = null;
        public string U_Fuelle { get; set; } = null;
        public string U_FuelleName { get; set; } = null;
        public string U_UvByMonCode { get; set; } = null;
        public string U_UvByMonName { get; set; } = null;
        public decimal U_PrjMonVol { get; set; }
        public decimal U_Price { get; set; }
        public string U_Observations { get; set; } = null;
    }
}

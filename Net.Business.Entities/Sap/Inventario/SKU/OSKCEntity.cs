using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("@FIB_OSKC")]
    public class OSKCEntity
    {
        [Key]
        public string Code { get; set; }
        public string U_Number { get; set; }
        public int U_SlpCode { get; set; }
        public string U_SlpName { get; set; }
        public string U_Status { get; set; }
        public string U_StatusName { get; set; }
        public DateTime U_DocDate { get; set; }
        public string U_ItemCodeBase { get; set; }
        public string U_ItemNameBase { get; set; }
        public int U_ItmsGrpCod { get; set; }
        public string U_ItmsGrpNam { get; set; }
        public string U_ItmsSGrpCod { get; set; }
        public string U_ItmsSGrpNam { get; set; }
        public string U_ItemName { get; set; }
        public string U_CardCode { get; set; }
        public string U_CardName { get; set; }
        public decimal U_Quantity { get; set; }
        public string U_UnitMsrCode { get; set; }
        public string U_UnitMsrName { get; set; }
        public decimal U_Wide { get; set; }
        public Int16 U_UnitCode { get; set; }
        public string U_UnitName { get; set; }
        public decimal U_Long { get; set; }
        public decimal U_GrMtSq { get; set; }
        public decimal U_ItemWeight { get; set; }
        public string U_ColorCode { get; set; }
        public string U_ColorName { get; set; }
        public string U_Laminate { get; set; }
        public string U_LaminateName { get; set; }
        public string U_LamTypCode { get; set; }
        public string U_LamTypName { get; set; }
        public string U_Linner { get; set; }
        public string U_LinnerName { get; set; }
        public decimal U_LinnWeight { get; set; }
        public string U_Print { get; set; }
        public string U_PrintName { get; set; }
        public string U_PrintColCode { get; set; }
        public string U_PrintColName { get; set; }
        public string U_Fuelle { get; set; }
        public string U_FuelleName { get; set; }
        public string U_UvByMonCode { get; set; }
        public string U_UvByMonName { get; set; }
        public decimal U_PrjMonVol { get; set; }
        public decimal U_Price { get; set; }
        public string U_Observations { get; set; }


        // FIND
        public DateTime StrDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Filtro { get; set; }
    }    
}

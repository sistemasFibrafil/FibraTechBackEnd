using System.Linq;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsCreateMassiveRequestDto
    {
        public bool IsEntrada { get; set; }
        public bool IsSalida { get; set; }
        public List<ItemsCreateMassiveLineRequestDto> Line { get; set; } = new List<ItemsCreateMassiveLineRequestDto>();

        public ItemsCreateMassiveEntity ReturnValue()
        {
            return new ItemsCreateMassiveEntity
            {
                IsEntrada = IsEntrada,
                IsSalida = IsSalida,
                Lines = Line.Select(line => new ItemsCreateMassiveLinesEntity
                {
                    ItemCode = line.ItemCode,
                    ItemName = line.ItemName,
                    U_FIB_ItemCode = line.U_FIB_ItemCode,
                    U_FIB_ItemName = line.U_FIB_ItemName,
                    ItmsGrpCod = line.ItmsGrpCod,
                    InvntItem = line.InvntItem,
                    SellItem = line.SellItem,
                    PrchseItem = line.PrchseItem,
                    WTLiable = line.WTLiable,
                    VatLiable = line.VatLiable,
                    IndirctTax = line.IndirctTax,
                    SalUnitMsr = line.SalUnitMsr,
                    BuyUnitMsr = line.BuyUnitMsr,
                    InvntryUom = line.InvntryUom,
                    DfltWH = line.DfltWH,
                    OnHand = line.OnHand,
                    TaxCodeAR = line.TaxCodeAR,
                    U_BPP_TIPEXIST = line.U_BPP_TIPEXIST,
                    U_BPP_TIPUNMED = line.U_BPP_TIPUNMED,
                    U_S_PartAranc1 = line.U_S_PartAranc1,
                    U_S_PartAranc2 = line.U_S_PartAranc2,
                    U_FIB_ECU = line.U_FIB_ECU,
                    U_S_CCosto = line.U_S_CCosto,
                    U_FIB_PESO = line.U_FIB_PESO,
                    U_FIB_SGRUP = line.U_FIB_SGRUP,
                    U_FIB_SGRUPO2 = line.U_FIB_SGRUPO2,
                    U_FIB_LINNEG = line.U_FIB_LINNEG,
                    U_UsrCreate = line.U_UsrCreate
                }).ToList()
            };
        }
    }

    public class ItemsCreateMassiveLineRequestDto
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? U_FIB_ItemName { get; set; }
        public string? U_FIB_ItemCode { get; set; }
        public short ItmsGrpCod { get; set; }
        public string? InvntItem { get; set; }
        public string? SellItem { get; set; }
        public string? PrchseItem { get; set; }
        public string? WTLiable { get; set; }
        public string? VatLiable { get; set; }
        public string? IndirctTax { get; set; }
        public string? SalUnitMsr { get; set; }
        public string? BuyUnitMsr { get; set; }
        public string? InvntryUom { get; set; }
        public string? DfltWH { get; set; }
        public decimal OnHand { get; set; }
        public string? TaxCodeAR { get; set; }
        public string? U_BPP_TIPEXIST { get; set; }
        public string? U_BPP_TIPUNMED { get; set; }
        public string? U_S_PartAranc1 { get; set; }
        public string? U_S_PartAranc2 { get; set; }
        public string? U_FIB_ECU { get; set; }
        public string? U_S_CCosto { get; set; }
        public decimal U_FIB_PESO { get; set; }
        public string? U_FIB_SGRUP { get; set; }
        public string? U_FIB_SGRUPO2 { get; set; }
        public string? U_FIB_LINNEG { get; set; }
        public int U_UsrCreate { get; set; }
    }
}

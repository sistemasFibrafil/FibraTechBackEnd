using System;
using System.Linq;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap
{
    public class OrdersCreateRequestDto
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string DocType { get; set; }

        public string U_FIB_DocStPkg { get; set; } = null;
        public string U_FIB_IsPkg { get; set; } = null;

        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int? CntctCode { get; set; } = 0;
        public string NumAtCard { get; set; }
        public string DocCur { get; set; } = null;
        public decimal DocRate { get; set; }

        public Int16 GroupNum { get; set; }

        public string ShipToCode { get; set; } = null;
        public string Address2 { get; set; } = null;
        public string PayToCode { get; set; } = null;
        public string Address { get; set; } = null;

        public string U_BPP_MDCT { get; set; } = null;
        public string U_BPP_MDRT { get; set; } = null;
        public string U_BPP_MDNT { get; set; } = null;
        public string U_FIB_AgencyToCode { get; set; } = null;
        public string U_BPP_MDDT { get; set; } = null;

        public string U_TipoFlete { get; set; } = null;
        public int? U_ValorFlete { get; set; } = 0;
        public decimal? U_FIB_TFLETE { get; set; } = 0;
        public decimal? U_FIB_IMPSEG { get; set; } = 0;
        public string U_FIB_PUERTO { get; set; } = null;

        public string U_STR_TVENTA { get; set; } = null;

        public int? SlpCode { get; set; } = -1;
        public string U_OrdenCompra { get; set; } = null;
        public string Comments { get; set; } = null;

        public decimal DiscPrcnt { get; set; } = 0;
        public decimal DiscSum { get; set; } = 0;
        public decimal VatSum { get; set; } = 0;
        public decimal DocTotal { get; set; } = 0;

        public int? U_UsrCreate { get; set; } = 0;

        public List<Orders1CreateRequestDto> Lines { get; set; } = new List<Orders1CreateRequestDto>();

        public OrdersCreateEntity ReturnValue()
        {
            var lines = Lines.Select(line => new Orders1CreateEntity
            {
                ItemCode = line.ItemCode,
                Dscription = line.Dscription,
                WhsCode = line.WhsCode,
                UnitMsr = line.UnitMsr,
                Quantity = line.Quantity,
                U_FIB_OpQtyPkg = line.U_FIB_OpQtyPkg,
                Currency = line.Currency,
                PriceBefDi = line.PriceBefDi,
                DiscPrcnt = line.DiscPrcnt,
                Price = line.Price,
                TaxCode = line.TaxCode,
                VatPrcnt = line.VatPrcnt,
                VatSum = line.VatSum,
                U_tipoOpT12 = line.U_tipoOpT12,
                LineTotal = line.LineTotal
            }).ToList();

            return new OrdersCreateEntity()
            {
                DocDate = DocDate,
                DocDueDate = DocDueDate,
                TaxDate = TaxDate,
                DocType = DocType,

                U_FIB_DocStPkg = U_FIB_DocStPkg,
                U_FIB_IsPkg = U_FIB_IsPkg,

                CardCode = CardCode,
                CardName = CardName,
                CntctCode = CntctCode,
                NumAtCard = NumAtCard,
                DocCur = DocCur,
                DocRate = DocRate,

                GroupNum = GroupNum,

                ShipToCode = ShipToCode,
                Address2 = Address2,
                PayToCode = PayToCode,
                Address = Address,

                U_BPP_MDCT = U_BPP_MDCT,
                U_BPP_MDRT = U_BPP_MDRT,
                U_BPP_MDNT = U_BPP_MDNT,
                U_FIB_AgencyToCode = U_FIB_AgencyToCode,
                U_BPP_MDDT = U_BPP_MDDT,

                U_TipoFlete = U_TipoFlete,
                U_ValorFlete = U_ValorFlete,
                U_FIB_TFLETE = U_FIB_TFLETE,
                U_FIB_IMPSEG = U_FIB_IMPSEG,
                U_FIB_PUERTO = U_FIB_PUERTO,

                U_STR_TVENTA = U_STR_TVENTA,

                SlpCode = SlpCode,
                Comments = Comments,

                DiscPrcnt = DiscPrcnt,
                DiscSum = DiscSum,
                VatSum = VatSum,
                DocTotal = DocTotal,

                U_UsrCreate = U_UsrCreate,

                Lines = lines
            };
        }
    }

    public class Orders1CreateRequestDto
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public string Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public string U_tipoOpT12 { get; set; } = null;
        public decimal LineTotal { get; set; }
    }
}

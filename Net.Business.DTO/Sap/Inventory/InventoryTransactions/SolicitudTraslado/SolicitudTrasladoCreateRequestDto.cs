using System;
using System.Linq;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap
{
    public class SolicitudTrasladoCreateRequestDto
    {
        public string ObjType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string U_FIB_IsPkg { get; set; } = null;
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int CntctCode { get; set; } = 0;
        public string Address { get; set; } = null;
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string U_FIB_TIP_TRAS { get; set; }
        public string U_BPP_MDMT { get; set; }
        public string U_BPP_MDTS { get; set; }
        public int? SlpCode { get; set; } = -1;
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrCreate { get; set; } = null;
        public List<SolicitudTrasladoDetalleCreateRequestDto> Lines { get; set; } = new List<SolicitudTrasladoDetalleCreateRequestDto>();

        public SolicitudTrasladoCreateEntity ReturnValue()
        {
            var lines = Lines.Select(line => new SolicitudTraslado1CreateEntity
            {
                ItemCode = line.ItemCode,
                Dscription = line.Dscription,
                FromWhsCod = line.FromWhsCod,
                WhsCode = line.WhsCode,
                U_tipoOpT12 = line.U_tipoOpT12,
                UnitMsr = line.UnitMsr,
                Quantity = line.Quantity,
                U_FIB_OpQtyPkg = line.U_FIB_OpQtyPkg
            }).ToList();

            return new SolicitudTrasladoCreateEntity()
            {
                DocDate = DocDate,
                DocDueDate = DocDueDate,
                TaxDate = TaxDate,
                U_FIB_IsPkg = U_FIB_IsPkg,
                CardCode = CardCode,
                CardName = CardName,
                CntctCode = CntctCode,
                Address = Address,
                Filler = Filler,
                ToWhsCode = ToWhsCode,
                U_FIB_TIP_TRAS = U_FIB_TIP_TRAS,
                U_BPP_MDMT = U_BPP_MDMT,
                U_BPP_MDTS = U_BPP_MDTS,
                SlpCode = SlpCode,
                JrnlMemo = JrnlMemo,
                Comments = Comments,
                U_UsrCreate = U_UsrCreate,
                Lines = lines
            };
        }
    }

    public class SolicitudTrasladoDetalleCreateRequestDto
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal U_FIB_OpQtyPkg { get; set; }

    }
}

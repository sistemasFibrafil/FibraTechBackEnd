using System;
using System.Linq;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap
{
    public class SolicitudTrasladoUpdateRequestDto
    {
        public int DocEntry { get; set; }
        public string ObjType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string U_FIB_IsPkg { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string U_FIB_TIP_TRAS { get; set; }
        public string U_BPP_MDMT { get; set; }
        public string U_BPP_MDTS { get; set; }
        public int SlpCode { get; set; } = -1;
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrUpdate { get; set; } = null;
        public List<SolicitudTraslado1UpdateDto> Lines { get; set; } = new List<SolicitudTraslado1UpdateDto>();

        public SolicitudTrasladoUpdateEntity ReturnValue()
        {
            var lines = Lines.Select(line => new SolicitudTraslado1UpdateEntity
            {
                DocEntry = line.DocEntry,
                LineNum = line.LineNum,
                ItemCode = line.ItemCode,
                LineStatus = line.LineStatus,
                Dscription = line.Dscription,
                FromWhsCod = line.FromWhsCod,
                WhsCode = line.WhsCode,
                U_tipoOpT12 = line.U_tipoOpT12,
                UnitMsr = line.UnitMsr,
                Quantity = line.Quantity,
                U_FIB_OpQtyPkg = line.U_FIB_OpQtyPkg,
                OpenQty = line.OpenQty,
                Record = line.Record,
            }).ToList();

            var value = new SolicitudTrasladoUpdateEntity()
            {
                DocEntry = DocEntry,
                DocDate = DocDate,
                DocDueDate = DocDueDate,
                TaxDate = TaxDate,
                U_FIB_IsPkg = U_FIB_IsPkg,
                Filler = Filler,
                ToWhsCode = ToWhsCode,
                U_FIB_TIP_TRAS = U_FIB_TIP_TRAS,
                U_BPP_MDMT = U_BPP_MDMT,
                U_BPP_MDTS = U_BPP_MDTS,
                SlpCode = SlpCode,
                JrnlMemo = JrnlMemo,
                Comments = Comments,
                U_UsrUpdate = U_UsrUpdate,
                Lines = lines
            };

            return value;
        }
    }

    public class SolicitudTraslado1UpdateDto
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal U_FIB_OpQtyPkg { get; set; }
        public int Record { get; set; } = 2;
    }
}

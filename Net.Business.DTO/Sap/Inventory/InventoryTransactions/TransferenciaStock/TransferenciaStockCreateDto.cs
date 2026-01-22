using System;
using System.Linq;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap
{
    public class TransferenciaStockCreateDto
    {
        public string ObjType { get; set; }
        public string U_BPP_MDTD { get; set; }
        public string U_BPP_MDSD { get; set; }
        public string U_BPP_MDCD { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int CntctCode { get; set; } = 0;
        public string Address { get; set; } = null;

        public string Filler { get; set; }
        public string ToWhsCode { get; set; }

        public string U_FIB_TIP_TRANS { get; set; }
        public string U_FIB_TIPDOC_TRA { get; set; }
        public string U_BPP_MDRT { get; set; }
        public string U_BPP_MDNT { get; set; }
        public string U_BPP_MDVC { get; set; }

        public string U_FIB_TIPDOC_COND { get; set; }
        public string U_FIB_NUMDOC_COD { get; set; }
        public string U_FIB_NOM_COND { get; set; }
        public string U_FIB_APE_COND { get; set; }
        public string U_BPP_MDFN { get; set; }
        public string U_BPP_MDFC { get; set; }

        public string U_FIB_TIP_TRAS { get; set; }
        public string U_BPP_MDMT { get; set; }
        public string U_BPP_MDTS { get; set; }

        public int? SlpCode { get; set; } = -1;
        public decimal U_FIB_NBULTOS { get; set; }
        public decimal U_FIB_KG { get; set; }
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrCreate { get; set; } = null;
        public List<TransferenciaStock1CreateDto> Lines { get; set; } = new List<TransferenciaStock1CreateDto>();
        public List<PickingUpdateDto> PickingLines { get; set; } = new List<PickingUpdateDto>();

        public TransferenciaStockCreateEntity ReturnValue()
        {
            var lines = Lines.Select(line => new TransferenciaStock1CreateEntity
            {
                BaseEntry = line.BaseEntry,
                BaseLine = line.BaseLine,
                BaseType = line.BaseType,
                U_FIB_FromPkg = line.U_FIB_FromPkg,
                ItemCode = line.ItemCode,
                Dscription = line.Dscription,
                FromWhsCod = line.FromWhsCod,
                WhsCode = line.WhsCode,
                U_tipoOpT12 = line.U_tipoOpT12,
                UnitMsr = line.UnitMsr,
                Quantity = line.Quantity,
                OpenQty = line.OpenQty,
                U_FIB_NBulto = line.U_FIB_NBulto,
                U_FIB_PesoKg = line.U_FIB_PesoKg
            }).ToList();
            var pickingLines = PickingLines.Select(line => new PickingUpdateEntity
            {
                DocEntry = line.DocEntry,
                U_BaseEntry = line.U_BaseEntry,
                U_BaseLine = line.U_BaseLine,
                U_Status = line.U_Status,
                U_UsrUpdate = line.U_UsrUpdate
            }).ToList();
            var value = new TransferenciaStockCreateEntity()
            {
                U_BPP_MDTD = U_BPP_MDTD,
                U_BPP_MDSD = U_BPP_MDSD,
                U_BPP_MDCD = U_BPP_MDCD,
                DocDate = DocDate,
                TaxDate = TaxDate,
                CardCode = CardCode,
                CardName = CardName,
                CntctCode = CntctCode,
                Address = Address,

                Filler = Filler,
                ToWhsCode = ToWhsCode,

                U_FIB_TIP_TRANS = U_FIB_TIP_TRANS,
                U_FIB_TIPDOC_TRA = U_FIB_TIPDOC_TRA,
                U_BPP_MDRT = U_BPP_MDRT,
                U_BPP_MDNT = U_BPP_MDNT,
                U_BPP_MDVC = U_BPP_MDVC,

                U_FIB_TIPDOC_COND = U_FIB_TIPDOC_COND,
                U_FIB_NUMDOC_COD = U_FIB_NUMDOC_COD,
                U_FIB_NOM_COND = U_FIB_NOM_COND,
                U_FIB_APE_COND = U_FIB_APE_COND,
                U_BPP_MDFN = U_BPP_MDFN,
                U_BPP_MDFC = U_BPP_MDFC,

                U_FIB_TIP_TRAS = U_FIB_TIP_TRAS,
                U_BPP_MDMT = U_BPP_MDMT,
                U_BPP_MDTS = U_BPP_MDTS,

                SlpCode = SlpCode,
                U_FIB_NBULTOS = U_FIB_NBULTOS,
                U_FIB_KG = U_FIB_KG,
                JrnlMemo = JrnlMemo,
                Comments = Comments,
                U_UsrCreate = U_UsrCreate,
                Lines = lines,
                PickingLines = pickingLines
            };
            return value;
        }
    }

    public class TransferenciaStock1CreateDto
    {
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public int BaseType { get; set; }
        public string U_FIB_FromPkg { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal U_FIB_NBulto { get; set; }
        public decimal U_FIB_PesoKg { get; set; }
    }

    public class PickingUpdateDto
    {
        public int DocEntry { get; set; }
        public int U_BaseEntry { get; set; }
        public int U_BaseLine { get; set; }
        public string U_Status { get; set; }
        public int U_UsrUpdate { get; set; }
    }
}

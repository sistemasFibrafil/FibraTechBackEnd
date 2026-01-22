using System;
using System.Collections.Generic;
using System.Linq;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TransferenciaStockUpdateDto
    {
        public int DocEntry { get; set; }
        public string ObjType { get; set; }
        public string U_BPP_MDTD { get; set; }
        public string U_BPP_MDSD { get; set; }
        public string U_BPP_MDCD { get; set; }
        public DateTime TaxDate { get; set; }

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

        public decimal U_FIB_NBULTOS { get; set; }
        public decimal U_FIB_KG { get; set; }
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrUpdate { get; set; } = null;

        public List<TransferenciaStock1UpdateDto> Lines { get; set; } = new List<TransferenciaStock1UpdateDto>();

        public TransferenciaStockUpdateEntity ReturnValue()
        {
            var value = new TransferenciaStockUpdateEntity()
            {
                DocEntry = DocEntry,
                U_BPP_MDTD = U_BPP_MDTD,
                U_BPP_MDSD = U_BPP_MDSD,
                U_BPP_MDCD = U_BPP_MDCD,
                TaxDate = TaxDate,

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

                U_FIB_NBULTOS = U_FIB_NBULTOS,
                U_FIB_KG = U_FIB_KG,
                JrnlMemo = JrnlMemo,
                Comments = Comments,
                U_UsrUpdate = U_UsrUpdate
            };
            return value;
        }
    }

    public class TransferenciaStock1UpdateDto
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
    }
}

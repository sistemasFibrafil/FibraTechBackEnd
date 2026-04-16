using System;
using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class StockTransfersUpdateRequestDto
    {
        public int DocEntry { get; set; }
        public string? ObjType { get; set; }

        public string? U_BPP_MDTD { get; set; }
        public string? U_BPP_MDSD { get; set; }
        public string? U_BPP_MDCD { get; set; }

        public DateTime TaxDate { get; set; }

        public string? CardCode { get; set; }

        public string? U_FIB_TIP_TRANS { get; set; }
        public string? U_FIB_COD_TRA { get; set; }
        public string? U_FIB_TIPDOC_TRA { get; set; }
        public string? U_BPP_MDRT { get; set; }
        public string? U_BPP_MDNT { get; set; }
        public string? U_BPP_MDVC { get; set; }

        public string? U_FIB_TIPDOC_COND { get; set; }
        public string? U_FIB_NUMDOC_COD { get; set; }
        public string? U_FIB_NOM_COND { get; set; }
        public string? U_FIB_APE_COND { get; set; }
        public string? U_BPP_MDFN { get; set; }
        public string? U_BPP_MDFC { get; set; }

        public string? U_FIB_TIP_TRAS { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDTS { get; set; }

        public int SlpCode { get; set; }

        public double U_FIB_NBULTOS { get; set; }
        public double U_FIB_KG { get; set; }

        public string? JrnlMemo { get; set; }
        public string? Comments { get; set; }

        public int U_UsrUpdate { get; set; }

        public List<StockTransfers1UpdateRequestDto> Lines { get; set; } = new List<StockTransfers1UpdateRequestDto>();
    }
}

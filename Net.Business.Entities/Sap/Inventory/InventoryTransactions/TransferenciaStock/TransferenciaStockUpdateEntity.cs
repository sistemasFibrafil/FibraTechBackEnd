using System;
namespace Net.Business.Entities.Sap
{
    public class TransferenciaStockUpdateEntity
    {
        public int DocEntry { get; set; }
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

        public decimal? U_FIB_NBULTOS { get; set; } = 0;
        public decimal? U_FIB_KG { get; set; } = 0;
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrUpdate { get; set; } = null;

    }
}

using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class OSKPEntity
    {
        public int DocEntry { get; set; }
        public string U_Number { get; set; }
        public string U_ItemName { get; set; }
        public string U_CardCode { get; set; }
        public string U_CardName { get; set; }
        public DateTime U_PrdStrDate { get; set; }
        public DateTime U_PrdEndDate { get; set; }
        public DateTime U_PrdEndHour { get; set; }
        public decimal U_RollWeight { get; set; }
        public string U_PrdForDetail { get; set; }
        public string U_PrdPresBale { get; set; }
        public string U_PrdFeaYes { get; set; }
        public string U_PrdFeaNo { get; set; }
        public string U_PrdFeaObs { get; set; }
        public string U_FeaQuaInd { get; set; }
        public string U_FeaQuaJus { get; set; }
        public DateTime? U_CosStrDate { get; set; } = null;
        public DateTime? U_CosEndDate { get; set; } = null;
        public DateTime? U_CosEndHour { get; set; } = null;
        public string U_CosDetail { get; set; }
        public string U_ValExcMar { get; set; }
        public string U_AprByExc { get; set; }
        public string U_Observations { get; set; }
        public string U_ItemCode { get; set; }
        public List<SKP1Entity> Line { get; set; } = new List<SKP1Entity>();

        public string Filtro { get; set; }
    }

    public class SKP1Entity
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string U_ProcessCode { get; set; }
        public string U_ProcessName { get; set; }
        public decimal U_Percentage1 { get; set; }
        public string U_ItemCode { get; set; }
        public string U_ItemName { get; set; }
        public decimal U_Percentage2 { get; set; }
    }
}

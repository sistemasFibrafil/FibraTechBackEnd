using System;
namespace Net.Business.Entities.Sap
{
    public class OSKPViewEntity
    {
        public int DocEntry { get; set; }
        public string U_Number { get; set; } = null;
        public string U_ItemCode { get; set; } = null;
        public string U_ItemName { get; set; } = null;
        public string U_CardCode { get; set; } = null;
        public string U_CardName { get; set; } = null;
        public DateTime? U_PrdStrDate { get; set; } = null;
        public DateTime? U_PrdEndDate { get; set; } = null;
        public DateTime? U_PrdEndHour { get; set; } = null;
        public decimal U_RollWeight { get; set; }
        public string U_PrdForDetail { get; set; } = null;
        public string U_PrdPresBale { get; set; } = null;
        public string U_PrdFeaYes { get; set; } = null;
        public string U_PrdFeaNo { get; set; } = null;
        public string U_PrdFeaObs { get; set; } = null;
        public string U_FeaQuaInd { get; set; } 
        public string U_FeaQuaJus { get; set; } = null;
        public DateTime? U_CosStrDate { get; set; } = null;
        public DateTime? U_CosEndDate { get; set; } = null;
        public DateTime? U_CosEndHour { get; set; } = null;
        public string U_CosDetail { get; set; } = null;
        public string U_ValExcMar { get; set; } = null;
        public string U_AprByExc { get; set; } = null;
        public string U_Observations { get; set; } = null;
    }
}

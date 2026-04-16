namespace Net.Business.Entities.SAPBusinessOne
{
    public class GeneralSettingsEntity
    {
        public string Code { get; set; } = string.Empty;
        public int? U_QtyDays { get; set; }
        public string? U_ActTranVal { get; set; }
        public decimal? U_StanComPerc { get; set; }
        public decimal? U_ComPrecNew { get; set; }
        public int? U_NewCustDays { get; set; }
        public string? U_WhsCodeSpaPar { get; set; }
        public int? U_CodGrpSuppNat { get; set; }
        public int? U_CodGrpSuppFor { get; set; }
        public int? U_CodGrpCustNat { get; set; } 
        public int? U_CodGrpCustFor { get; set; }
    }
}

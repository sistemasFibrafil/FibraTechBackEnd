using System;
namespace Net.Business.Entities.Sap
{
    public class PagoRecibidoEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    public class PagoRecibidoByFilterEntity
    {
        public DateTime CourtDate { get; set; }
        public string BusinessPartnerGroup { get; set; }
        public string Customer { get; set; }
    }
    public class CobranzaCarteraVencidaByFilterSapEntity
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string GroupName { get; set; }
        public decimal CreditLine { get; set; }
        public string SlpName { get; set; }
        public int NumeroAsiento { get; set; }
        public int NumeroSAP { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Comments { get; set; }
        public string Segment_0 { get; set; }
        public string CondicionPago { get; set; }
        public string Moneda { get; set; }
        public decimal SaldoSOL { get; set; }
        public decimal SaldoUSD { get; set; }
        public decimal SaldoSYS { get; set; }
        public decimal De_0_15_Dias { get; set; }
        public decimal De_16_30_Dias { get; set; }
        public decimal De_31_60_Dias { get; set; }
        public decimal Mas_60_Dias { get; set; }
    }
}

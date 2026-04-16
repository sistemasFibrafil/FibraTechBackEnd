using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class StockTransfersPrintEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int DocNum { get; set; }
        public string Codigo { get; set; }
        public string Version { get; set; }
        public string Vigencia { get; set; }
        public DateTime TaxDate { get; set; }
        public string SedeOrigen { get; set; }
        public string SedeDestino { get; set; }
        public string TipoTraslado { get; set; }
        public string Comments { get; set; }
    }
}

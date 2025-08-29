using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class TransferenciaStockSapEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    public class TransferenciaStockQrySapEntity
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

    public class TransferenciaStockDetalleQrySapEntity
    {
        public int Line { get; set; }
        public int NumSolicitud { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public decimal Quantity { get; set; }
    }
}

using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class TransferenciaStockEntity
    {
        public int Id { get; set; }
        public string ObjType { get; set; } = "67";
        public int DocEntry { get; set; } = 0;
        public int DocNum { get; set; } = 0;
        public string DocStatus { get; set; } = null;
        public string TipDocumento { get; set; }
        public string SerDocumento { get; set; }
        public string NumDocumento { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int CntctCode { get; set; } = 0;
        public string Address { get; set; } = null;
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }

        public string CodTipTransporte { get; set; }
        public string CodTipDocTransportista { get; set; }
        public string NumTipoDocTransportista { get; set; }
        public string NomTransportista { get; set; }
        public string NumPlaVehTransportista { get; set; }

        public string CodTipDocConductor { get; set; }
        public string NumTipoDocConductor { get; set; }
        public string NomConductor { get; set; }
        public string ApeConductor { get; set; }
        public string NomComConductor { get; set; }
        public string NumLicConductor { get; set; }

        public string CodTipTraslado { get; set; }
        public string CodMotTraslado { get; set; }
        public string CodTipSalida { get; set; }

        public int SlpCode { get; set; }
        public decimal NumBulto { get; set; }
        public decimal TotKilo { get; set; }
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public string CodStatusSunat { get; set; } = null;
        public string NomStatusSunat { get; set; } = null;
        public string DesSunat { get; set; } = null;
        public string NotSunat { get; set; } = null;
        public int QtyRding { get; set; } = 0;

        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
        public int? IdUsuarioClose { get; set; } = null;
        public List<TransferenciaStockDetalleEntity> Linea { get; set; } = new List<TransferenciaStockDetalleEntity>();

    }

    public class TransferenciaStockDetalleEntity
    {
        public int Id { get; set; }
        public int Line { get; set; }
        public int IdLectura { get; set; }
        public int IdBase { get; set; }
        public int LineBase { get; set; }
        public string ObjType { get; set; } = "67";
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string BaseType { get; set; } = "-1";
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string Read { get; set; } = "N";
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string CodTipOperacion { get; set; }
        public string NomTipOperacion { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
        public int? IdUsuarioClose { get; set; } = null;
    }
}

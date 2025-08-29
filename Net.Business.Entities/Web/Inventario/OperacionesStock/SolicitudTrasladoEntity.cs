using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class SolicitudTrasladoEntity
    {
        public int Id { get; set; }
        public string ObjType { get; set; } = "1250000001";
        public int DocEntry { get; set; } = 0;
        public int DocNum { get; set; } = 0;
        public string DocStatus { get; set; } = null;
        public string DocStatusRding { get; set; } = null;
        public string DocManClose { get; set; } = null;
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string Read { get; set; } = null;
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int CntctCode { get; set; } = 0;
        public string Address { get; set; } = null;
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string CodTipTraslado { get; set; }
        public string CodMotTraslado { get; set; }
        public string CodTipSalida { get; set; }
        public int SlpCode { get; set; }
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
        public int? IdUsuarioClose { get; set; } = null;
        public List<SolicitudTrasladoDetalleEntity> Linea { get; set; } = new List<SolicitudTrasladoDetalleEntity>();
    }

    public class SolicitudTrasladoDetalleEntity
    {
        public int Id { get; set; }
        public int Line { get; set; }
        public string ObjType { get; set; } = "1250000001";
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string LineStatusRding { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQtyRding { get; set; }
        public decimal OpenQty { get; set; }
        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
        public int? IdUsuarioClose { get; set; } = null;
        public int Record { get; set; } = 2;
    }


    public class SolicitudTrasladoToTransferenciaEntity
    {
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int CntctCode { get; set; } = 0;
        public string Address { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string CodTipTraslado { get; set; }
        public string CodMotTraslado { get; set; }
        public string CodTipSalida { get; set; }
        public int SlpCode { get; set; }
        public string JrnlMemo { get; set; }
        public string Comments { get; set; }
        public List<SolicitudTrasladoDetalleToTransferenciaEntity> Linea { get; set; } = new List<SolicitudTrasladoDetalleToTransferenciaEntity>();
    }

    public class SolicitudTrasladoDetalleToTransferenciaEntity
    {
        public int Id { get; set; }
        public int IdBase { get; set; }
        public int LineBase { get; set; }
        public string BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string Return { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string Barcode { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string CodTipOperacion { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
    }
}

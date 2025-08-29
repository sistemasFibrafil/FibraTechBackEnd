using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class LecturaEntity
    {
        public int Id { get; set; }
        public int IdBase { get; set; }
        public int LineBase { get; set; }
        public string NumBase { get; set; }
        public string BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public int BaseNum { get; set; }
        public string TargetType { get; set; }
        public int TrgetEntry { get; set; }
        public int TrgetLine { get; set; }
        public string DocStatus { get; set; }
        public string Return { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string Barcode { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; } = null;
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal QtyRead { get; set; }
        public decimal EngQtyRead { get; set; }
        public decimal DedQtyRead { get; set; }
        public decimal Peso { get; set; }
        public int IdUsuarioCreate { get; set; }
    }

    public class LecturaCopyToTransferenciaFindEntity
    {
        public int IdBase { get; set; }
        public string BaseType { get; set; }
        public List<LecturaCopyToTransferenciaDetalleFindEntity> Linea { get; set; } = new List<LecturaCopyToTransferenciaDetalleFindEntity>();
    }

    public class LecturaCopyToTransferenciaDetalleFindEntity
    {
        public int IdBase { get; set; }
        public int LineBase { get; set; }
        public string BaseType { get; set; }
        public string Read { get; set; }
        public string Return { get; set; }
    }


    public class LecturaCopyToTransferenciaEntity
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
        public List<LecturaCopyToTransferenciaDetalleEntity1> Linea1 { get; set; } = new List<LecturaCopyToTransferenciaDetalleEntity1>();
        public List<LecturaCopyToTransferenciaDetalleEntity2> Linea2 { get; set; } = new List<LecturaCopyToTransferenciaDetalleEntity2>();
    }

    public class LecturaCopyToTransferenciaDetalleEntity1
    {
        public int Id { get; set; }
        public int IdBase { get; set; }
        public int LineBase { get; set; }
        public string BaseType { get; set; } 
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string Read { get; set; }
        public string Return { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string Barcode { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string CodTipOperacion { get; set; }
        public string NomTipOperacion { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal Bulto { get; set; }
        public decimal Peso { get; set; }
    }

    public class LecturaCopyToTransferenciaDetalleEntity2
    {
        public int IdBase { get; set; }
        public int LineBase { get; set; }
        public string BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string Read { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string CodTipOperacion { get; set; }
        public string NomTipOperacion { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal Bulto { get; set; }
        public decimal Peso { get; set; }
    }

    public class PackingListEntity
    {
        public string TargetType { get; set; }
        public int TrgetEntry { get; set; }
        public int TrgetLine { get; set; }
        public string CardName { get; set; }
        public string Contenedor { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public List<PackingListDetalleEntity> Linea { get; set; } = new List<PackingListDetalleEntity>();
    }

    public class PackingListDetalleEntity
    {
        public int Id { get; set; }
        public string Barcode1 { get; set; } = null;
        public string Barcode2 { get; set; } = null;
        public string Barcode3 { get; set; } = null;
        public string Barcode4 { get; set; } = null;
        public int TotalItem { get; set; }
        public decimal PesoTotal { get; set; }
    }
}

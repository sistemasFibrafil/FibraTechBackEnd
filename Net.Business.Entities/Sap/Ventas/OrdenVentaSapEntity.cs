using System;
namespace Net.Business.Entities.Sap
{
    public class OrdenVentaSapEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    public class OrdenVentaSapByFechaEntity
    {
        public int DocEntry { get; set; }
        public string CodTipDocumento { get; set; }
        public string NomTipDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public int NumeroPedido { get; set; }
        public string NumeroOrdenCompra { get; set; }
        public int? NumeroFactura { get; set; } = null;
        public int LineNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string LineStatus { get; set; }
        public string CodStatus { get; set; }
        public string NomStatus { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string PaisDF { get; set; }
        public string DepartamentoDF { get; set; }
        public string ProvinciaDF { get; set; }
        public string CiudadDF { get; set; }
        public string PaisDD { get; set; }
        public string DepartamentoDD { get; set; }
        public string ProvinciaDD { get; set; }
        public string CiudadDD { get; set; }
        public string UbigeoDD { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodLinNegocio { get; set; }
        public string NomLinNegocio { get; set; }
        public int CodGpoArticulo { get; set; }
        public string NomGpoArticulo { get; set; }
        public string NomSubGpoArticulo { get; set; }
        public string NomSubGpoArticulo2 { get; set; }
        public string Medida { get; set; }
        public string Color { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string BuyUnitMsr { get; set; }
        public string SalUnitMsr { get; set; }
        public string InvntryUom { get; set; }
        public decimal Stock { get; set; }
        public decimal Pendiente { get; set; }
        public decimal Solicitado { get; set; }
        public decimal Disponible { get; set; }
        public decimal StockProduccion { get; set; }
        public decimal PendienteProduccion { get; set; }
        public decimal SolicitadoProduccion { get; set; }
        public decimal DisponibleProduccion { get; set; }
        public decimal Quantity { get; set; }
        public decimal RolloPedido { get; set; }
        public decimal Peso { get; set; }
        public decimal KgPedido { get; set; }
        public decimal ToneladaPedida { get; set; }
        public decimal OpenQty { get; set; }
        public decimal LineTotEarring { get; set; }
        public decimal RolloPendiente { get; set; }
        public decimal KgPendiente { get; set; }
        public decimal ToneladaPendiente { get; set; }
        public decimal DelivrdQty { get; set; }
        public string Currency { get; set; }
        public string DocCur { get; set; }
        public Decimal Rate { get; set; }
        public Decimal Price { get; set; }
        public Decimal LineTotal { get; set; }
        public Decimal TotalFrgn { get; set; }
        public Decimal TotalSumSy { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalFC { get; set; }
        public decimal DocTotalSy { get; set; }
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
        public int CodConPago { get; set; }
        public string NomConPago { get; set; }
        public string IdDivision { get; set; }
        public string Division { get; set; }
        public string IdSector { get; set; }
        public string Sector { get; set; }
        public decimal PesoPromedioKg { get; set; }
        public int DiasAntiguedad { get; set; }
        public int DiasAtraso { get; set; }
        public string DiasVenc { get; set; }
        public string CodOriCliente { get; set; }
        public string NomOriCliente { get; set; }
        public string Sede { get; set; }
    }

    public class OrdenVentaCsigSapEntity
    {
        public int DocNum { get; set; }
        public DateTime? ShipDate { get; set; }
        public string ItemName { get; set; }
        public string CardName { get; set; }
        public string NomTrasportista { get; set; }
        public string DirTransportista { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime? FechaDespacho { get; set; }
        public int Status { get; set; }
        public int Cumplimiento { get; set; }
        public decimal PesoApox { get; set; }
    }

    public class OrdenVentaSodimacSapEntity
    {
        public int DocEntry { get; set; }
        public string NumOrdenCompra { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int CntctCode { get; set; }
        public string CntctName { get; set; }
        public string Address2 { get; set; }
    }

    public class OrdenVentaSeguimientoFindEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BusinessPartnerGroup { get; set; }
        public string SalesEmployee { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
    }
}

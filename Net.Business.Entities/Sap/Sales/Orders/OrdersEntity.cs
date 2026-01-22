using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class OrdersEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public string DocType { get; set; }
        public string DocStatus { get; set; } = null;

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        

        public string U_FIB_DocStPkg { get; set; } = null;
        public string U_FIB_IsPkg { get; set; } = null;

        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int? CntctCode { get; set; } = 0;
        public string NumAtCard { get; set; }
        public string DocCur { get; set; } = null;
        public decimal DocRate { get; set; }

        public Int16 GroupNum { get; set; }

        public string PayToCode { get; set; } = null;
        public string Address { get; set; } = null;
        public string ShipToCode { get; set; } = null;
        public string Address2 { get; set; } = null;

        public string U_BPP_MDCT { get; set; } = null;
        public string U_BPP_MDRT { get; set; } = null;
        public string U_BPP_MDNT { get; set; } = null;
        public string U_FIB_AgencyToCode { get; set; } = null;
        public string U_BPP_MDDT { get; set; } = null;

        public string U_TipoFlete { get; set; } = null;
        public int? U_ValorFlete { get; set; } = 0;
        public decimal? U_FIB_TFLETE { get; set; } = 0;
        public decimal? U_FIB_IMPSEG { get; set; } = 0;
        public string U_FIB_PUERTO { get; set; } = null;

        public string U_STR_TVENTA { get; set; } = null;

        public int? SlpCode { get; set; } = -1;
        public string Comments { get; set; } = null;

        public decimal DiscPrcnt { get; set; } = 0;
        public decimal DiscSum { get; set; } = 0;
        public decimal DiscSumSy { get; set; } = 0;
        public decimal VatSum { get; set; } = 0;
        public decimal VatSumSy { get; set; } = 0;
        public decimal DocTotal { get; set; } = 0;
        public decimal DocTotalSy { get; set; } = 0;


        // 🔗 1 → N (ORDR → RDR1)
        public ICollection<Orders1Entity> Lines { get; set; } = new List<Orders1Entity>();
    }

    public class Orders1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; } = null;
        public int? BaseLine { get; set; } = null;
        public string U_FIB_LinStPkg { get; set; } = null;
        public string U_FIB_FromPkg { get; set; } = null;
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string AcctCode { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public string Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumSy { get; set; }
        public string U_tipoOpT12 { get; set; } = null;
        public decimal LineTotal { get; set; }
        public decimal TotalSumSy { get; set; }

        // 🔗 N → 1 (OITM → Items)
        public ItemsEntity Item { get; set; }

        // 🔗 N → 1 (PRQ1 → ChartOfAccounts)
        public ChartOfAccountsEntity ChartOfAccounts { get; set; }


        // 🔗 N → 1 (RDR1 → TipoOperacion)
        public TipoOperacionEntity TipoOperacion { get; set; }
    }

    public class OrdersFechaEntity
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

    public class OrdersSeguimientoFindEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BusinessPartnerGroup { get; set; }
        public string SalesEmployee { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }
    }
}

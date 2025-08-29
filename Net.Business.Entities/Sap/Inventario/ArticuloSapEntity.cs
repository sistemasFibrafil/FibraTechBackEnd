using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("OITM")]
    public class ArticuloSapEntity
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string NomGrupo { get; set; }
        public string NomSubGrupo { get; set; }
        public string NomSubGrupo2 { get; set; }
        public string DfltWH { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string BuyUnitMsr { get; set; }
        public string SalUnitMsr { get; set; }
        public string InvntryUom { get; set; }
        public decimal OnHand { get; set; }
        public decimal IsCommited { get; set; }
        public decimal OnOrder { get; set; }
        public decimal Available { get; set; }
        public decimal PesoArticulo { get; set; }
        public decimal PesoPromedioKg { get; set; }
        public decimal PesoKg { get; set; }
        public DateTime? FecProduccion { get; set; } = null;
    }


    public class MovimientoStockSapByFechaSedeFindEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string TypeMovement { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }
    }

    public class MovimientoStockSapByFechaSedeEntity
    {
        public string NomTipoMovimiento { get; set; }
        public int? NumeroGuiaSAP { get; set; }
        public string NumeroGuiaSUNAT { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Usuario { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Sede { get; set; }
        public string CentroCosto { get; set; }
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public decimal Bulto { get; set; }
        public decimal TotalKg { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Quantity { get; set; }
        public int? NumeroPedido { get; set; }
        public DateTime? FechaPedido { get; set; }
        public int? NumeroFcturaSAP { get; set; }
        public string NumeroFcturaSUNAT { get; set; }
        public string NomTransportista { get; set; }
        public string RucTransportista { get; set; }
        public string PlacaTransportista { get; set; }
        public string NomConductor { get; set; }
        public string LincenciaConductor { get; set; }
    }

    public class ArticuloVentaStockByGrupoSubGrupo
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string NomGrupo { get; set; }
        public string NomSubGrupo { get; set; }
        public string NomSubGrupo2 { get; set; }
        public string UnidadVenta { get; set; }
        public decimal Stock { get; set; }
        public decimal Comprometido { get; set; }
        public decimal Solicitado { get; set; }
        public decimal Disponible { get; set; }
        public decimal PesoPromedioKg { get; set; }
    }

    public class ArticuloVentaByGrupoSubGrupoEstado
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string NomGrupo { get; set; }
        public string NomSubGrupo { get; set; }
        public string NomSubGrupo2 { get; set; }
        public string NomEstado { get; set; }
        public string UnidadVenta { get; set; }
        public decimal PesoItem { get; set; }
        public decimal PesoPromedioKg { get; set; }
    }

    public class ArticuloSapForSodimacBySkuEntity
    {
        public List<ArticuloSapForSodimacBySkuItemEntity> Linea { get; set; } = new List<ArticuloSapForSodimacBySkuItemEntity>();
    }

    public class ArticuloSapForSodimacBySkuItemEntity
    {
        public int Line1 { get; set; }
        public int Line2 { get; set; }
        public int NumLocal { get; set; }
        public string NomLocal { get; set; }
        public bool IsOriente { get; set; } = false;
        public string CodEstado { get; set; }
        public string ItemCode { get; set; }
        public string Sku { get; set; }
        public string Dscription { get; set; }
        public string DscriptionLarga { get; set; }
        public string Ean { get; set; }
        public decimal Quantity { get; set; }
    }

    public class ArticuloDocumentoSapEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string DfltWH { get; set; }
        public string BuyUnitMsr { get; set; }
        public string SalUnitMsr { get; set; }
        public string InvntryUom { get; set; }
        public decimal OnHand { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal OpenQtyRead { get; set; }
        public string Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal LineTotal { get; set; }
        public decimal VatSum { get; set; }
    }
}

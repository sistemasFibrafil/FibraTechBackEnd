using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class ItemsEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Int16 ItmsGrpCod { get; set; }
        public string InvntItem { get; set; }
        public string SellItem { get; set; }
        public string PrchseItem { get; set; }
        public string WTLiable { get; set; }
        public string VatLiable { get; set; }
        public string IndirctTax { get; set; }
        public string frozenFor { get; set; }
        public string SalUnitMsr { get; set; }
        public string BuyUnitMsr { get; set; }
        public string InvntryUom { get; set; }
        public decimal SalPackUn { get; set; }
        public string DfltWH { get; set; }
        public decimal? OnHand { get; set; } = 0;
        public string TaxCodeAR { get; set; }
        public string U_FIB_ItemCode { get; set; }
        public string U_FIB_ItemName { get; set; }
        public string U_BPP_TIPEXIST { get; set; }
        public string U_BPP_TIPUNMED { get; set; }
        public string U_S_PartAranc1 { get; set; }
        public string U_S_PartAranc2 { get; set; }
        public string U_FIB_ECU { get; set; }
        public string U_S_CCosto { get; set; }
        public decimal? U_FIB_PESO { get; set; } = 0;
        public string U_FIB_SGRUP { get; set; }
        public string U_FIB_SGRUPO2 { get; set; }
        public string U_FIB_LINNEG { get; set; }
        public int? U_UsrCreate { get; set; } = 0;

        // 🔹 Relación N:1 (lado N) - Items donde este almacén es default
        public WarehousesEntity DefaultWarehouse { get; set; }

        // 🔹 Relación 1:N — un Items tiene muchos ItemWarehouseInfo
        public ICollection<ItemWarehouseInfoEntity> ItemWarehouseInfo { get; set; }


        // 🔹 Relación 1:N con ITM1
        public ICollection<PriceListsEntity> PriceLists { get; set; }
    }

    public class ArticuloReporteEntity
    {
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


    public class ArticuloMovimientoStockFindEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string TypeMovement { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }
    }

    public class MovimientoStockByFechaSedeEntity
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

    public class ArticuloSodimacBySkuEntity
    {
        public List<ArticuloForSodimacBySkuItemEntity> Linea { get; set; } = new List<ArticuloForSodimacBySkuItemEntity>();
    }

    public class ArticuloForSodimacBySkuItemEntity
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

    public class ArticuloDocumentoEntity
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

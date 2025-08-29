using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class FacturaVentaSapEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardName { get; set; }

        public List<VentaResumenSapByFechaGrupoEntity> VentaResumen1 { get; set; }
        public List<VentaResumenSapByFechaGrupoEntity> VentaResumen2 { get; set; }
        public List<VentaResumenSapByFechaGrupoEntity> VentaResumen3 { get; set; }
    }

    public class VentaProyeccionSapByFechaEntity
    {
        public int SlpCode { get; set; }
        public string SlpName { get; set; }

        public decimal VentaMesAnioAnterior { get; set; }
        public decimal CuotaMesAnioAnterior { get; set; }
        public decimal VariacionMesAnioAnterior { get; set; }
        public decimal AvanceMesAnioAnterior { get; set; }

        public decimal VentaAnioAnterior { get; set; }
        public decimal CuotaAnioAnterior { get; set; }
        public decimal VariacionAnioAnterior { get; set; }
        public decimal AvanceAnioAnterior { get; set; }

        public decimal VentaMesAnterior { get; set; }
        public decimal CuotaMesAnterior { get; set; }
        public decimal VariacionMesAnterior { get; set; }
        public decimal AvanceMesAnterior { get; set; }

        public decimal VentaMesActual { get; set; }
        public decimal CuotaMesActual { get; set; }
        public decimal VariacionMesActual { get; set; }
        public decimal AvanceMesActual { get; set; }

        public decimal VentaAnioActual { get; set; }
        public decimal CuotaAnioActual { get; set; }
        public decimal VariacionAnioActual { get; set; }
        public decimal AvanceAnioActual { get; set; }
    }

    public class VentaResumenSapByFechaGrupoEntity
    {
        public string NomVendedor { get; set; }
        public string NomGrupo { get; set; }
        public string ItemName { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Cantidad { get; set; }
        public decimal TotalItemUSD { get; set; }
    }

    public class VentaSapByFilterFindEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SalesEmployee { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }
    }

    public class VentaSapByFilterCodeEntity
    {
        public string UnidadNegocio { get; set; }
        public string EmpresaAsociada { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Division { get; set; }
        public string Sector { get; set; }
        public string Pais { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Cuidad { get; set; }
        public string TipoDocumento { get; set; }
        public DateTime FecContabilizacion { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroGuia { get; set; }
        public int? NumeroPedido { get; set; }
        public DateTime? FechaPedido { get; set; }
        public string NomVendedor { get; set; }
        public string NomCondicionPago { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string NomGrupo { get; set; }
        public decimal Forcast { get; set; }
        public string NomSubGrupo { get; set; }
        public string NomSubGrupo2 { get; set; }
        public string Medida { get; set; }
        public string Color { get; set; }
        public string Porcentaje { get; set; }
        public string UnidadMedida { get; set; }
        public decimal PesoItem { get; set; }
        public decimal PesoPromedioKg { get; set; }
        public decimal PesoVentaKg { get; set; }
        public decimal Peso { get; set; }
        public decimal Cantidad { get; set; }
        public decimal RolloVendido { get; set; }
        public decimal KgVendido { get; set; }
        public decimal ToneladaVendida { get; set; }
        public string CodMoneda { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioKg { get; set; }
        public decimal CostoSOL { get; set; }
        public decimal CostoUSD { get; set; }
        public decimal TotalCostoItemSOL { get; set; }
        public decimal TotalCostoItemUSD { get; set; }
        public decimal TotalItemSOL { get; set; }
        public decimal TotalItemUSD { get; set; }
        public string Sede { get; set; }
        public string Ubigeo { get; set; }
    }


    public class FacturaVentaSapByFilterFindEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Customer { get; set; }
    }
    public class FacturaVentaSapByFechaEntity
    {
        public string CardName { get; set; }
        public DateTime FecContabilizacion { get; set; }
        public DateTime FecVencimiento { get; set; }
        public int DiaVencido { get; set; }
        public string NumeroDocumento { get; set; }
        public string NomVendedor { get; set; }
        public string CodMoneda { get; set; }
        public decimal Total { get; set; }
        public decimal Cobrado { get; set; }
        public decimal Saldo { get; set; }
    }

}

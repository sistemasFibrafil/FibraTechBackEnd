using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class SopEntity
    {
        public int Id { get; set; }
        public int CodYear { get; set; }
        public int CodMonth { get; set; }
        public string NamMonth { get; set; }
        public int CodWeek { get; set; }
        public string NamWeek { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int IdUsuarioCreate { get; set; }
        public int IdUsuarioUpdate { get; set; }
        public List<SopDetalleEntity> Linea { get; set; } = new List<SopDetalleEntity>();
    }

    public class SopDetalleEntity
    {
        public int Id { get; set; }
        public int Line { get; set; }
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public int? Order { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public string CodTipDocumento { get; set; }
        public string NomTipDocumento { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CodOriCliente { get; set; }
        public string NomOriCliente { get; set; }
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodLinNegocio { get; set; }
        public string NomLinNegocio { get; set; }
        public int CodGpoArticulo { get; set; }
        public string NomGpoArticulo { get; set; }
        public string SalUnitMsr { get; set; }
        public decimal Stock { get; set; }
        public decimal QtyEarring { get; set; }
        public decimal PesoPromedioKg { get; set; }
        public decimal KgEarring { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotEarring { get; set; }
        public int CodConPago { get; set; }
        public string NomConPago { get; set; }
        public DateTime? FecEntFinal { get; set; } = null;
        public DateTime? FecEntProdProceso { get; set; } = null;
        public int Record { get; set; } = 2;
        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
    }
}

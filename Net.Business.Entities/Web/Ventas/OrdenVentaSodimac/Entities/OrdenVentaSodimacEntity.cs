using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimacEntity
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? NumOrdenCompra { get; set; }
        public string? DocStatus { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int CntctCode { get; set; } = 0;
        public string? CntctName { get; set; } = null;
        public string? Address { get; set; } = null;
        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
        public List<OrdenVentaSodimacLinesEntity> Lines { get; set; } = new List<OrdenVentaSodimacLinesEntity>();
    }
}

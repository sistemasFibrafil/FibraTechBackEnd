using System;
using System.Collections.Generic;
using Net.Business.Entities.Web;

namespace Net.Business.DTO.Web
{
    public class SopCreateRequestDto
    {
        public int Id { get; set; }
        public int CodYear { get; set; }
        public int CodMonth { get; set; }
        public int CodWeek { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int IdUsuarioCreate { get; set; }
        public List<SopDetalleCreateRequestDto> Linea { get; set; } = new List<SopDetalleCreateRequestDto>();

        public SopEntity ReturnValue()
        {
            var value = new SopEntity()
            {
                Id = Id,
                CodYear = CodYear,
                CodMonth = CodMonth,
                CodWeek = CodWeek,
                Name = Name,
                Comments = Comments,
                IdUsuarioCreate = IdUsuarioCreate,
            };
            foreach (var linea in Linea)
            {
                value.Linea.Add(new SopDetalleEntity()
                {
                    Id = linea.Id,
                    DocEntry = linea.DocEntry,
                    LineNum = linea.LineNum,
                    DocNum = linea.DocNum,
                    DocDate = linea.DocDate,
                    CodTipDocumento = linea.CodTipDocumento,
                    NomTipDocumento = linea.NomTipDocumento,
                    CardCode = linea.CardCode,
                    CardName = linea.CardName,
                    CodOriCliente = linea.CodOriCliente,
                    NomOriCliente = linea.NomOriCliente,
                    SlpCode = linea.SlpCode,
                    SlpName = linea.SlpName,
                    ItemCode = linea.ItemCode,
                    ItemName = linea.ItemName,
                    CodLinNegocio = linea.CodLinNegocio,
                    NomLinNegocio = linea.NomLinNegocio,
                    CodGpoArticulo = linea.CodGpoArticulo,
                    NomGpoArticulo = linea.NomGpoArticulo,
                    SalUnitMsr = linea.SalUnitMsr,
                    Stock = linea.Stock,
                    QtyEarring = linea.QtyEarring,
                    PesoPromedioKg = linea.PesoPromedioKg,
                    KgEarring = linea.KgEarring,
                    Price = linea.Price,
                    LineTotEarring = linea.LineTotEarring,
                    CodConPago = linea.CodConPago,
                    NomConPago = linea.NomConPago,
                    IdUsuarioCreate = linea.IdUsuarioCreate,
                });
            }

            return value;
        }
    }

    public class SopDetalleCreateRequestDto
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
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
        public int? IdUsuarioCreate { get; set; } = null;
    }
}

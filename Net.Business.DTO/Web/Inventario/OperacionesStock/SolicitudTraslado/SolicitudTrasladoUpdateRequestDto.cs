using System;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Business.DTO.Web
{
    public class SolicitudTrasladoUpdateRequestDto
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string Read { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string CodTipTraslado { get; set; }
        public string CodMotTraslado { get; set; }
        public string CodTipSalida { get; set; }
        public int SlpCode { get; set; }
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int CodSede { get; set; } = 0;
        public bool IsNotRestAlmacen { get; set; } = false;
        public int? IdUsuarioUpdate { get; set; } = null;
        public List<SolicitudTrasladoDetalleUpdateRequestDto> Linea { get; set; } = new List<SolicitudTrasladoDetalleUpdateRequestDto>();

        public SolicitudTrasladoEntity ReturnValue()
        {
            var value = new SolicitudTrasladoEntity()
            {
                Id = Id,
                DocEntry = DocEntry,
                DocDate = DocDate,
                DocDueDate = DocDueDate,
                TaxDate = TaxDate,
                Read = Read,
                Filler = Filler,
                ToWhsCode = ToWhsCode,
                CodTipTraslado = CodTipTraslado,
                CodMotTraslado = CodMotTraslado,
                CodTipSalida = CodTipSalida,
                SlpCode = SlpCode,
                JrnlMemo = JrnlMemo,
                Comments = Comments,
                IdUsuarioUpdate = IdUsuarioUpdate,
            };

            foreach (var linea in Linea)
            {
                value.Linea.Add(new SolicitudTrasladoDetalleEntity()
                {
                    Id = linea.Id,
                    Line = linea.Line,
                    DocEntry = linea.DocEntry,
                    LineNum = linea.LineNum,
                    LineStatus = linea.LineStatus,
                    ItemCode = linea.ItemCode,
                    Dscription = linea.Dscription,
                    FromWhsCod = linea.FromWhsCod,
                    WhsCode = linea.WhsCode,
                    UnitMsr = linea.UnitMsr,
                    Quantity = linea.Quantity,
                    OpenQtyRding = linea.OpenQtyRding,
                    OpenQty = linea.OpenQty,
                    IdUsuarioCreate = linea.IdUsuarioCreate,
                    IdUsuarioUpdate = linea.IdUsuarioUpdate,
                    Record = linea.Record,
                });
            }

            return value;
        }
    }

    public class SolicitudTrasladoDetalleUpdateRequestDto
    {
        public int Id { get; set; }
        public int Line { get; set; }
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal OpenQtyRding { get; set; }
        public int? IdUsuarioCreate { get; set; } = null;
        public int? IdUsuarioUpdate { get; set; } = null;
        public int Record { get; set; } = 2;
    }
}

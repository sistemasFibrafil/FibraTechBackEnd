using Net.Business.Entities.Web;
using System;
using System.Collections.Generic;
namespace Net.Business.DTO.Web
{
    public class TransferenciaStockUpdateRequestDto
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CodTipTransporte { get; set; }
        public string CodTipDocTransportista { get; set; }
        public string NumTipoDocTransportista { get; set; }
        public string NomTransportista { get; set; }
        public string NumPlaVehTransportista { get; set; }

        public string CodTipDocConductor { get; set; }
        public string NumTipoDocConductor { get; set; }
        public string NomConductor { get; set; }
        public string ApeConductor { get; set; }
        public string NomComConductor { get; set; }
        public string NumLicConductor { get; set; }

        public string CodTipTraslado { get; set; }
        public string CodMotTraslado { get; set; }
        public string CodTipSalida { get; set; }

        public int SlpCode { get; set; }
        public decimal NumBulto { get; set; }
        public decimal TotKilo { get; set; }
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int CodSede { get; set; } = 0;
        public bool IsNotRestAlmacen { get; set; } = false;
        public int? IdUsuarioUpdate { get; set; } = null;
        public List<TransferenciaStockDetalleUpdateRequestDto> Linea { get; set; } = new List<TransferenciaStockDetalleUpdateRequestDto>();

        public TransferenciaStockEntity ReturnValue()
        {
            var value = new TransferenciaStockEntity()
            {
                Id = Id,
                DocEntry = DocEntry,
                DocDueDate = DocDueDate,

                CodTipTransporte = CodTipTransporte,
                CodTipDocTransportista = CodTipDocTransportista,
                NumTipoDocTransportista = NumTipoDocTransportista,
                NomTransportista = NomTransportista,
                NumPlaVehTransportista = NumPlaVehTransportista,

                CodTipDocConductor = CodTipDocConductor,
                NumTipoDocConductor = NumTipoDocConductor,
                NomConductor = NomConductor,
                ApeConductor = ApeConductor,
                NomComConductor = NomComConductor,
                NumLicConductor = NumLicConductor,

                CodTipTraslado = CodTipTraslado,
                CodMotTraslado = CodMotTraslado,
                CodTipSalida = CodTipSalida,

                SlpCode = SlpCode,
                NumBulto = NumBulto,
                TotKilo = TotKilo,
                JrnlMemo = JrnlMemo,
                Comments = Comments,
                IdUsuarioUpdate = IdUsuarioUpdate,
            };
            foreach (var linea in Linea)
            {
                value.Linea.Add(new TransferenciaStockDetalleEntity()
                {
                    Id = linea.Id,
                    Line = linea.Line,
                    DocEntry = linea.DocEntry,
                    LineNum = linea.LineNum,
                    ItemCode = linea.ItemCode,
                    Dscription = linea.Dscription,
                    FromWhsCod = linea.FromWhsCod,
                    WhsCode = linea.WhsCode,
                    CodTipOperacion = linea.CodTipOperacion,
                    UnitMsr = linea.UnitMsr,
                    Quantity = linea.Quantity,
                    OpenQty = linea.OpenQty
                });
            }

            return value;
        }
    }

    public class TransferenciaStockDetalleUpdateRequestDto
    {
        public int Id { get; set; }
        public int Line { get; set; }
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string CodTipOperacion { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
    }
}

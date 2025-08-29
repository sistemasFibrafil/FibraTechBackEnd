using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Business.DTO.Web
{
    public class SerieNumeracionActionRequestDto
    {
        public string CodSerieNumeracion { get; set; }
        public string TipDocumento { get; set; }
        public string SerDocumento { get; set; }
        public string NumDocumento { get; set; }
        public string MaxNumDocumento { get; set; }
        public int CodSede { get; set; }
        public int CodFormulario { get; set; }
        public int IdUsuario { get; set; }
        public List<SerieNumeracionActionDto> Linea { get; set; } = new List<SerieNumeracionActionDto>();

        public SerieNumeracionEntity ReturnValue()
        {
            var value = new SerieNumeracionEntity()
            {
                CodSerieNumeracion = CodSerieNumeracion,
                TipDocumento = TipDocumento,
                SerDocumento = SerDocumento,
                NumDocumento = NumDocumento,
                MaxNumDocumento = MaxNumDocumento,
                CodSede = CodSede,
                CodFormulario = CodFormulario,
                IdUsuario = IdUsuario,
            };

            foreach (var linea in Linea)
            {
                value.Linea.Add(new SerieNumeracionActionEntity()
                {
                    CodSerieNumeracion = linea.CodSerieNumeracion,
                    TipDocumento = linea.TipDocumento,
                    SerDocumento = linea.SerDocumento,
                    NumDocumento = linea.NumDocumento,
                    MaxNumDocumento = linea.MaxNumDocumento,
                    CodSede = linea.CodSede,
                    CodFormulario = linea.CodFormulario,
                    IdUsuario = linea.IdUsuario,
                    Record = linea.Record,
                });
            }

            return value;
        }
    }

    public class SerieNumeracionActionDto
    {
        public string CodSerieNumeracion { get; set; }
        public string TipDocumento { get; set; }
        public string SerDocumento { get; set; }
        public string NumDocumento { get; set; }
        public string MaxNumDocumento { get; set; }
        public int CodSede { get; set; }
        public int CodFormulario { get; set; }
        public int IdUsuario { get; set; }
        public int Record { get; set; } = 2;
    }
}

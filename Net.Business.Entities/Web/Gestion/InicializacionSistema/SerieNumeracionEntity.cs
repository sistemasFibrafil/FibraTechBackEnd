using System.Collections.Generic;

namespace Net.Business.Entities.Web
{
    public class SerieNumeracionEntity
    {
        public string CodSerieNumeracion { get; set; }
        public string TipDocumento { get; set; }
        public string SerDocumento { get; set; }
        public string NumDocumento { get; set; }
        public string MaxNumDocumento { get; set; }
        public int CodSede { get; set; }
        public int CodFormulario { get; set; }
        public string NomFormulario { get; set; }
        public int IdUsuario { get; set; }
        public int Record { get; set; } = 2;
        public List<SerieNumeracionActionEntity> Linea { get; set; } = new List<SerieNumeracionActionEntity>();
    }

    public class SerieNumeracionActionEntity
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

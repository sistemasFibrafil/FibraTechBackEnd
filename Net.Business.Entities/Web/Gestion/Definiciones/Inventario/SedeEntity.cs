using System.Collections.Generic;

namespace Net.Business.Entities.Web
{
    public class SedeEntity
    {
        public int CodSede { get; set; }
        public string NomSede { get; set; }
        public int IdUsuario { get; set; }
        public int Record { get; set; } = 2;
        public List<SedeActionEntity> Linea { get; set; } = new List<SedeActionEntity>();
    }

    public class SedeActionEntity
    {
        public int CodSede { get; set; }
        public string NomSede { get; set; }
        public int IdUsuario { get; set; }
        public int Record { get; set; } = 2;
    }
}

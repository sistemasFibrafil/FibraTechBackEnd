using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Business.DTO.Web
{
    public class SedeActionRequestDto
    {
        public int CodSede { get; set; }
        public string NomSede { get; set; }
        public int IdUsuario { get; set; }
        public List<SedeActionDto> Linea { get; set; } = new List<SedeActionDto>();

        public SedeEntity ReturnValue()
        {
            var value = new SedeEntity()
            {
                CodSede = CodSede,
                NomSede = NomSede,
                IdUsuario = IdUsuario,
            };

            foreach (var linea in Linea)
            {
                value.Linea.Add(new SedeActionEntity()
                {
                    CodSede = linea.CodSede,
                    NomSede = linea.NomSede,
                    IdUsuario = linea.IdUsuario,
                    Record = linea.Record,
                });
            }

            return value;
        }
    }

    public class SedeActionDto
    {
        public int CodSede { get; set; }
        public string NomSede { get; set; }
        public int IdUsuario { get; set; }
        public int Record { get; set; } = 2;
    }
}

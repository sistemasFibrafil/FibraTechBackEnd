using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKPFindByFiltroRequestDto
    {
        public string Filtro { get; set; }

        public OSKPEntity ReturnValue()
        {
            return new OSKPEntity
            {
                Filtro = Filtro
            };
        }
    }
}

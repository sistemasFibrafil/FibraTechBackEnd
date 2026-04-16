using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKCFindByFiltroRequestDto
    {
        public string Filtro { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                Filtro = Filtro
            };
        }
    }
}

using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class OSKPFindByFiltroRequestDto
    {
        public string Filtro { get; set; }

        public OSKPEntity ReturnValue()
        {
            return new OSKPEntity
            {
                Filtro = this.Filtro
            };
        }
    }
}

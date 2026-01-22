using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class OSKCFindByFiltroRequestDto
    {
        public string Filtro { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                Filtro = this.Filtro
            };
        }
    }
}

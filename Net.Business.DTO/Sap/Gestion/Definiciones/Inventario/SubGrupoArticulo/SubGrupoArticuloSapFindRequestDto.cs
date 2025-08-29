using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class SubGrupoArticuloSapFindRequestDto
    {
        public string Name { get; set; }

        public SubGrupoArticuloSapEntity ReturnValue()
        {
            return new SubGrupoArticuloSapEntity
            {
                Name = Name
            };
        }
    }
}

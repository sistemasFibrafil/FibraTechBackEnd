using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class SubGrupoArticuloSapFindRequestDto
    {
        public string Name { get; set; }

        public SubGrupoArticuloEntity ReturnValue()
        {
            return new SubGrupoArticuloEntity
            {
                Name = Name
            };
        }
    }
}

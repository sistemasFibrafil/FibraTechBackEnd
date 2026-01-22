using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class ProcesoFindRequestDto
    {
        public string Name { get; set; }

        public ProcesoEntity ReturnValue()
        {
            return new ProcesoEntity
            {
                Name = Name
            };
        }
    }
}

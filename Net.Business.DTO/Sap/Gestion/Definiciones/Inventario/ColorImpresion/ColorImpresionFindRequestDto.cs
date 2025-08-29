using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class ColorImpresionFindRequestDto
    {
        public string Name { get; set; }

        public ColorImpresionEntity ReturnValue()
        {
            return new ColorImpresionEntity
            {
                Name = Name
            };
        }
    }
}

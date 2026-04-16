using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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

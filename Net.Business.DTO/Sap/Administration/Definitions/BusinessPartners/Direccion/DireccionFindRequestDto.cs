using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class DireccionFindRequestDto
    {
        public string CardCode { get; set; }
        public string Address { get; set; }
        public string AdresType { get; set; }
        public DireccionEntity ReturnValue()
        {
            return new DireccionEntity
            {
                CardCode = CardCode,
                AdresType = AdresType,
                Address = Address,
            };
        }
    }
}

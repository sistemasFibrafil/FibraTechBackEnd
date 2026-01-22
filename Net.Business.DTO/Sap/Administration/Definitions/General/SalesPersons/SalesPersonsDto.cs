using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class SalesPersonsDto
    {
        public string SlpName { get; set; }

        public SalesPersonsEntity ReturnValue()
        {
            return new SalesPersonsEntity
            {
                SlpName = SlpName
            };
        }
    }
}

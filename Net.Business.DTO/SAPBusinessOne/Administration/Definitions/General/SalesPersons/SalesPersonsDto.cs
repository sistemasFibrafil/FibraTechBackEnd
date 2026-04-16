using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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

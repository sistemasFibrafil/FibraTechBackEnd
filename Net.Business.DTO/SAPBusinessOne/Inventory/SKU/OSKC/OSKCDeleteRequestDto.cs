using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKCDeleteRequestDto
    {
        public string Code { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                Code = Code,
            };
        }
    }
}

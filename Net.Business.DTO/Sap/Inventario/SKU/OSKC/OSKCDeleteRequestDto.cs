using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class OSKCDeleteRequestDto
    {
        public string Code { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                Code = this.Code,
            };
        }
    }
}

using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class OSKPDeleteRequestDto
    {
        public int DocEntry { get; set; }

        public OSKPEntity ReturnValue()
        {
            return new OSKPEntity
            {
                DocEntry = this.DocEntry,
            };
        }
    }
}

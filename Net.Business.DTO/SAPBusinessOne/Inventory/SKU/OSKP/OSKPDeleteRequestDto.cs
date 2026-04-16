using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKPDeleteRequestDto
    {
        public int DocEntry { get; set; }

        public OSKPEntity ReturnValue()
        {
            return new OSKPEntity
            {
                DocEntry = DocEntry,
            };
        }
    }
}

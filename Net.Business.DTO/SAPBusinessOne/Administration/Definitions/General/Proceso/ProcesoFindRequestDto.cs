using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ProcesoFindRequestDto
    {
        public string Name { get; set; }

        public ProcessesEntity ReturnValue()
        {
            return new ProcessesEntity
            {
                Name = Name
            };
        }
    }
}

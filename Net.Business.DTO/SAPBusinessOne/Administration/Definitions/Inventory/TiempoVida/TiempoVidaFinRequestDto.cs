using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TiempoVidaFinRequestDto
    {
        public string Name { get; set; }

        public TiempoVidaEntity ReturnValue()
        {
            return new TiempoVidaEntity
            {
                Name = Name
            };
        }
    }
}

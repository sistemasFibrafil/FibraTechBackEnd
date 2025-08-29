using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
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

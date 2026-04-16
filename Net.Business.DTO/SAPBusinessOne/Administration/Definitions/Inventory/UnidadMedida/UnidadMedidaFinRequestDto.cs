using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class UnidadMedidaFinRequestDto
    {
        public string Name { get; set; }

        public UnidadMedidaEntity ReturnValue()
        {
            return new UnidadMedidaEntity
            {
                Name = Name
            };
        }
    }
}

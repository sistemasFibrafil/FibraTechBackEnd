using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
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

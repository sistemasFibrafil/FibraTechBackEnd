using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class EmpleadoVentaSapDto
    {
        public string SlpName { get; set; }

        public EmpleadoVentaSapEntity ReturnValue()
        {
            return new EmpleadoVentaSapEntity
            {
                SlpName = SlpName
            };
        }
    }
}

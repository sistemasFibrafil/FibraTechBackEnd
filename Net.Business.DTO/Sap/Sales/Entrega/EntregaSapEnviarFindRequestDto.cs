using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class DtoEntregaSapEnviarFindRequest
    {
        public int DocEntry { get; set; }

        public EntregaSapEntity RetornaEntregaEnviar()
        {
            return new EntregaSapEntity
            {
                DocEntry = DocEntry
            };
        }
    }
}

using Net.Business.Entities.Sap;
namespace Net.Business.DTO
{
    public class TipoOperacionFinRequestDto
    {
        public string U_descrp { get; set; }

        public TipoOperacionSapEntity ReturnValue()
        {
            return new TipoOperacionSapEntity
            {
                U_descrp = U_descrp
            };
        }
    }
}

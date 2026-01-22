using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class SolicitudTrasladoCloseRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrUpdate { get; set; }

        public SolicitudTrasladoCloseEntity ReturnValue()
        {
            return new SolicitudTrasladoCloseEntity()
            {
                DocEntry = DocEntry,
                U_UsrUpdate = U_UsrUpdate,
            };
        }
    }
}

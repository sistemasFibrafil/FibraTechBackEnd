using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class CampoDefinidoUsuarioFindRequestDto
    {
        public string TableID { get; set; }
        public string AliasID { get; set; }

        public CampoDefinidoUsuarioEntity ReturnValue()
        {
            return new CampoDefinidoUsuarioEntity
            {
                TableID = TableID,
                AliasID = AliasID
            };
        }
    }
}

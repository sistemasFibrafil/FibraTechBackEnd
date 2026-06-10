using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Filter;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OperationsTypesFinRequestDto
    {
        public string? TipoOperacion { get; set; }

        public OperationsTypesFilterEntity ReturnValue()
        {
            return new OperationsTypesFilterEntity
            {
                TipoOperacion = TipoOperacion
            };
        }
    }
}

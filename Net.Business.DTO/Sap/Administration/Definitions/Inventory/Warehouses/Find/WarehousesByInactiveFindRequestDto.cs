using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class WarehousesByInactiveFindRequestDto
    {
        public string Inactive { get; set; }

        public WarehousesEntity ReturnValue()
        {
            return new WarehousesEntity
            {
                Inactive = Inactive
            };
        }
    }
}

using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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

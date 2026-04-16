using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsStockGeneralViewFilterRequestDto
    {
        public string? WhsCode { get; set; }
        public bool ExcluirInactivo { get; set; }
        public bool ExcluirSinStock { get; set; }

        public ItemsStockGeneralViewFilterEntity ReturnValue()
        {
            return new ItemsStockGeneralViewFilterEntity
            {
                WhsCode = WhsCode,
                ExcluirInactivo = ExcluirInactivo,
                ExcluirSinStock = ExcluirSinStock
            };
        }
    }
}

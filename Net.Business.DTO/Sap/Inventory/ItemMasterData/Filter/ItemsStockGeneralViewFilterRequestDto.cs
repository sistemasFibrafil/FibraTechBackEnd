using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class ItemsStockGeneralViewFilterRequestDto
    {
        public string WhsCode { get; set; }
        public bool ExcluirInactivo { get; set; }
        public bool ExcluirSinStock { get; set; }

        public ItemsStockGeneralViewFilterEntity ReturnValue()
        {
            return new ItemsStockGeneralViewFilterEntity
            {
                WhsCode = this.WhsCode,
                ExcluirInactivo = this.ExcluirInactivo,
                ExcluirSinStock = this.ExcluirSinStock
            };
        }
    }
}

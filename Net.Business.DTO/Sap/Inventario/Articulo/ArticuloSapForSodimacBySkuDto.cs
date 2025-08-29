using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap.Inventario.Articulo
{
    public class ArticuloSapForSodimacBySkuDto
    {
        public List<ArticuloSapForSodimacBySkuItemDto> Linea { get; set; } = new List<ArticuloSapForSodimacBySkuItemDto>();

        public ArticuloSapForSodimacBySkuEntity ReturnValue()
        {
            var value = new ArticuloSapForSodimacBySkuEntity();
            foreach (var item in Linea)
            {
                value.Linea.Add(new ArticuloSapForSodimacBySkuItemEntity()
                {
                    Line2 = item.Line,
                    NumLocal = item.NumLocal,
                    NomLocal = item.NomLocal,
                    CodEstado = item.CodEstado,
                    Sku = item.Sku,
                    DscriptionLarga = item.DscriptionLarga,
                    Ean = item.Ean,
                    Quantity = item.Quantity
                });
            }

            return value;
        }

    }

    public class ArticuloSapForSodimacBySkuItemDto
    {
        public int Line { get; set; }
        public int NumLocal { get; set; }
        public string NomLocal { get; set; }
        public string CodEstado { get; set; }
        public string Sku { get; set; }
        public string DscriptionLarga { get; set; }
        public string Ean { get; set; }
        public decimal Quantity { get; set; }
    }
}

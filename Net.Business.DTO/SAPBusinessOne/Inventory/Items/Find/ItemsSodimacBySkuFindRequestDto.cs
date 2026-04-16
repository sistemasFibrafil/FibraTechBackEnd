using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsSodimacBySkuFindRequestDto
    {
        public List<ArticuloSodimacBySkuItemDto> Linea { get; set; } = new List<ArticuloSodimacBySkuItemDto>();

        public ArticuloSodimacBySkuEntity ReturnValue()
        {
            var value = new ArticuloSodimacBySkuEntity();
            foreach (var item in Linea)
            {
                value.Linea.Add(new ArticuloForSodimacBySkuItemEntity()
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

    public class ArticuloSodimacBySkuItemDto
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

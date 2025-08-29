using System.Collections.Generic;
using Net.Business.Entities.Web;

namespace Net.Business.DTO.Web
{
    public class OrdenVentaSodimacUpdateRequestDto
    {
        public int Id { get; set; }
        public int IdUsuarioUpdate { get; set; }
        public List<OrdenVentaDetalleSodimacUpdateRequestDto> Item { get; set; } = new List<OrdenVentaDetalleSodimacUpdateRequestDto>();

        public OrdenVentaSodimacEntity ReturnValue()
        {
            var value = new OrdenVentaSodimacEntity()
            {
                Id = this.Id,
                IdUsuarioUpdate = this.IdUsuarioUpdate
            };

            foreach (var item in Item)
            {
                value.Item.Add(new OrdenVentaDetalleSodimacEntity()
                {
                    Id = item.Id,
                    Line1 = item.Line1,
                    IsOriente = item.IsOriente,
                });
            }

            return value;
        }
    }

    public class OrdenVentaDetalleSodimacUpdateRequestDto
    {
        public int Id { get; set; }
        public int Line1 { get; set; }
        public bool IsOriente { get; set; } = false;
    }
}

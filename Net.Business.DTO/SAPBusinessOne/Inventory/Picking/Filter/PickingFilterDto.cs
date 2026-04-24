using System;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Filter;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Filter
{
    public class PickingFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ObjType { get; set; }
        public string? Status { get; set; }
        public string? SearchText { get; set; }

        public PickingFilterEntity ReturnValue()
        {
            return new PickingFilterEntity()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                ObjType = ObjType,
                Status = Status,
                SearchText = SearchText
            };
        }
    }
}

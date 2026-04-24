using System.Linq;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Find
{
    public class PickingCopyToFindDto
    {
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public List<PickingCopyTo1FindDto> Lines { get; set; } = new List<PickingCopyTo1FindDto>();

        public PickingCopyToFindEntity ReturnValue()
        {
            var lines = Lines.Select(line => new PickingCopyTo1FindEntity
            {
                U_BaseEntry = line.U_BaseEntry,
                U_BaseType = line.U_BaseType,
                U_BaseLine = line.U_BaseLine,
                U_FIB_IsPkg = line.U_FIB_IsPkg,
            }).ToList();

            var value = new PickingCopyToFindEntity()
            {
                U_BaseEntry = U_BaseEntry,
                U_BaseType = U_BaseType,
                Lines = lines
            };
            
            return value;
        }
    }

    public class PickingCopyTo1FindDto
    {
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public int U_BaseLine { get; set; }
        public string? U_FIB_IsPkg { get; set; }
    }
}

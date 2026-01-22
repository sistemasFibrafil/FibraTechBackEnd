using System.Linq;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap
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
                U_BaseEntry = this.U_BaseEntry,
                U_BaseType = this.U_BaseType,
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
        public string U_FIB_IsPkg { get; set; }
    }
}

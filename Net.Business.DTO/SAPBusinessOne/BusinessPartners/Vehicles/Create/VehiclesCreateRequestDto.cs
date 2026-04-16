using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class VehiclesCreateRequestDto
    {
        public string? CardCode { get; set; }
        public List<VehiclesCreate1RequestDto> Lines { get; set; } = new List<VehiclesCreate1RequestDto>();
    }
}

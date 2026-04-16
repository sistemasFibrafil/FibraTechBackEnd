using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class DriversCreateRequestDto
    {
        public string? CardCode { get; set; }
        public List<Drivers1CreateRequestDto> Lines { get; set; } = new List<Drivers1CreateRequestDto>();
    }
}

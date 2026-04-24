using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Create
{
    public class DriversCreateRequestDto
    {
        public string? CardCode { get; set; }
        public List<DriversLinesCreateRequestDto> Lines { get; set; } = [];
    }
}

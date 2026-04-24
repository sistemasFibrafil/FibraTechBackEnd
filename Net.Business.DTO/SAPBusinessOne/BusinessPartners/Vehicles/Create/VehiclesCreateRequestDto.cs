using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create
{
    public class VehiclesCreateRequestDto
    {
        public string? CardCode { get; set; }
        public List<VehiclesLinesCreateRequestDto> Lines { get; set; } = [];
    }
}

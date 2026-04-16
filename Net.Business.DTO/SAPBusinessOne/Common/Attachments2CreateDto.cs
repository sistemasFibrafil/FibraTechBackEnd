using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class Attachments2CreateDto
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesCreateDto> Lines { get; set; } = new List<Attachments2LinesCreateDto>();
    }
}

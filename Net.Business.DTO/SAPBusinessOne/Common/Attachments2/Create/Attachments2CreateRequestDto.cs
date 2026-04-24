using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Create
{
    public class Attachments2CreateRequestDto
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesCreateRequestDto> Lines { get; set; } = [];
    }
}

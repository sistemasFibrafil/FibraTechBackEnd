using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Update
{
    public class Attachments2UpdateRequestDto
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesUpdateRequestDto> Lines { get; set; } = [];
    }
}

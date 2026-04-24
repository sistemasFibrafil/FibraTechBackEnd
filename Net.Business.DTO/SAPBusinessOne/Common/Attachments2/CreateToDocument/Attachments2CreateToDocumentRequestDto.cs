using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne.Common.Attachments2.CreateToDocument
{
    public class Attachments2CreateToDocumentRequestDto
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesCreateToDocumentRequestDto> Lines { get; set; } = [];
    }
}

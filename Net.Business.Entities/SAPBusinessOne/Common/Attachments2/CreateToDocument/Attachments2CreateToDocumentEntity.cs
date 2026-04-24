using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.CreateToDocument
{
    public class Attachments2CreateToDocumentEntity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesCreateToDocumentEntity> Lines { get; set; } = [];
    }
}

using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Entities
{
    public class Attachments2Entity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesEntity> Lines { get; set; } = [];
    }
}

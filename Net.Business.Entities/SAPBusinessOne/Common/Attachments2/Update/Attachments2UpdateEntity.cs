using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Update
{
    public class Attachments2UpdateEntity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesUpdateEntity> Lines { get; set; } = [];
    }
}

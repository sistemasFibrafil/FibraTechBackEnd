using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Query
{
    public class Attachments2QueryEntity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesQueryEntity> Lines { get; set; } = [];
    }
}

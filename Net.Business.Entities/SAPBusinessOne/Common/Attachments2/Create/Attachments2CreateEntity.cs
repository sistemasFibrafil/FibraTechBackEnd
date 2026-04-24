using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Create
{
    public class Attachments2CreateEntity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesCreateEntity> Lines { get; set; } = [];
    }
}

using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class Attachments2CreateEntity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesCreateEntity> Lines { get; set; } = new List<Attachments2LinesCreateEntity>();
    }
}

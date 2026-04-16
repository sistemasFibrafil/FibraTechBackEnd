using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class Attachments2Entity
    {
        public int AbsEntry { get; set; }
        public List<Attachments2LinesEntity> Lines { get; set; } = new List<Attachments2LinesEntity>();
    }
}

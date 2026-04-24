using System;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Update
{
    public class Attachments2LinesUpdateEntity
    {
        public int AbsEntry { get; set; }
        public string? SrcPath { get; set; }
        public string? TrgtPath { get; set; }
        public string? FileName { get; set; }
        public string? FileExt { get; set; }
        public DateTime Date { get; set; }
        public int Record { get; set; }
    }
}

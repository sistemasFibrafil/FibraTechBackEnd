using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class Attachments2LinesEntity
    {
        public int AbsEntry { get; set; }
        public int Line { get; set; }
        public string? srcPath { get; set; }
        public string? trgtPath { get; set; }
        public string? FileName { get; set; }
        public string? FileExt { get; set; }
        public DateTime Date { get; set; }
    }
}

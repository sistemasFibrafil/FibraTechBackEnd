using System;
namespace Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Create
{
    public class Attachments2LinesCreateEntity
    {
        public string? SrcPath { get; set; }
        public string? TrgtPath { get; set; }
        public string? FileName { get; set; }
        public string? FileExt { get; set; }
        public DateTime Date { get; set; }
    }
}

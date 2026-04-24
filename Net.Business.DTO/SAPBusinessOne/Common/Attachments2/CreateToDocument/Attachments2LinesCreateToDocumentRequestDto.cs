using System;
namespace Net.Business.DTO.SAPBusinessOne.Common.Attachments2.CreateToDocument
{
    public class Attachments2LinesCreateToDocumentRequestDto
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

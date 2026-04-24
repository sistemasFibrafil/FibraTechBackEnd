using System;
namespace Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Create
{
    public class Attachments2LinesCreateRequestDto
    {
        public string? SrcPath { get; set; }
        public string? TrgtPath { get; set; }
        public string? FileName { get; set; }
        public string? FileExt { get; set; }
        public DateTime Date { get; set; }
        public int Record { get; set; }
    }
}

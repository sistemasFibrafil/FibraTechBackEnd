using System;
using Microsoft.AspNetCore.Http;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class Attachments2LinesCreateEntity
    {
        public string? SrcPath { get; set; }
        public string? TrgtPath { get; set; }
        public string? FileName { get; set; }
        public string? FileExt { get; set; }
        public DateTime Date { get; set; }
        public IFormFile? File { get; set; }
    }
}

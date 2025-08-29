using System;
using System.Collections.Generic;
namespace Net.Business.Entities
{
    public class FilterRequestEntity
    {
        public DateTime? Dat1 { get; set; } = null;
        public DateTime? Dat2 { get; set; } = null;
        public int Id1 { get; set; } = 0;
        public int Id2 { get; set; } = 0;
        public int Id3 { get; set; } = 0;
        public int Id4 { get; set; } = 0;
        public int Id5 { get; set; } = 0;
        public string Cod1 { get; set; } = "";
        public string Cod2 { get; set; } = "";
        public string Cod3 { get; set; } = "";
        public string Cod4 { get; set; } = "";
        public string Cod5 { get; set; } = "";
        public int Val1 { get; set; } = 0;
        public int Val2 { get; set; } = 0;
        public int Val3 { get; set; } = 0;
        public int Val4 { get; set; } = 0;
        public decimal Val5 { get; set; } = 0;
        public decimal Dec1 { get; set; } = 0;
        public decimal Dec2 { get; set; } = 0;
        public decimal Dec3 { get; set; } = 0;
        public decimal Dec4 { get; set; } = 0;
        public decimal Dec5 { get; set; } = 0;
        public string Text1 { get; set; } = "";
        public string Text2 { get; set; } = "";
        public string Text3 { get; set; } = "";
        public string Text4 { get; set; } = "";
        public string Text5 { get; set; } = "";
        public List<string> List1 { get; set; } = null;
    }
}

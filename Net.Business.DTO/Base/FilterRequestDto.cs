using System;
using Net.Business.Entities;
namespace Net.Business.DTO
{
    public class FilterRequestDto
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
        public int Val5 { get; set; } = 0;
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

        public FilterRequestEntity ReturnValue()
        {
            return new FilterRequestEntity
            {
                Dat1 = this.Dat1,
                Dat2 = this.Dat2,
                Id1 = this.Id1,
                Id2 = this.Id2,
                Id3 = this.Id3,
                Id4 = this.Id4,
                Id5 = this.Id5,
                Cod1 = this.Cod1,
                Cod2 = this.Cod2,
                Cod3 = this.Cod3,
                Cod4 = this.Cod4,
                Cod5 = this.Cod5,
                Val1 = this.Val1,
                Val2 = this.Val2,
                Val3 = this.Val3,
                Val4 = this.Val4,
                Val5 = this.Val5,
                Dec1 = this.Dec1,
                Dec2 = this.Dec2,
                Dec3 = this.Dec3,
                Dec4 = this.Dec4,
                Dec5 = this.Dec5,
                Text1 = this.Text1,
                Text2 = this.Text2,
                Text3 = this.Text3,
                Text4 = this.Text4,
                Text5 = this.Text5,
            };
        }
    }
}

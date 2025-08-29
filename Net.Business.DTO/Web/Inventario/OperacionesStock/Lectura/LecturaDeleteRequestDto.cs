using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class LecturaDeleteRequestDto
    {
        public int Id { get; set; } = 0;
        public string BaseType { get; set; } = null;
        public int BaseEntry { get; set; } = 0;
        public int BaseLine { get; set; } = 0;
        public string Return { get; set; } = null;
        public string DocStatus { get; set; } = null;

        public LecturaEntity ReturnValue()
        {
            return new LecturaEntity()
            {
                Id = this.Id,
                BaseType = this.BaseType,
                BaseEntry = this.BaseEntry,
                BaseLine = this.BaseLine,
                Return = this.Return,
                DocStatus = this.DocStatus
            };
        }
    }
}

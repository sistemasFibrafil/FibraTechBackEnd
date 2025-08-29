using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class LecturaCreateRequestDto
    {
        public string BaseType { get; set; }
        public int BaseEntry { get; set; }
        public string FromWhsCod { get; set; }
        public string Barcode { get; set; }
        public int IdUsuarioCreate { get; set; }

        public LecturaEntity ReturnValue()
        {
            return new LecturaEntity()
            {
                BaseType = this.BaseType,
                BaseEntry = this.BaseEntry,
                FromWhsCod = this.FromWhsCod,
                Barcode = this.Barcode,
                IdUsuarioCreate = IdUsuarioCreate,
            };
        }
    }
}

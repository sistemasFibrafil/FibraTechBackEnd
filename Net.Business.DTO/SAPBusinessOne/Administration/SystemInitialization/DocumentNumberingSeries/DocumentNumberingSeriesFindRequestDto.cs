using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class DocumentNumberingSeriesFindRequestDto
    {
        public string? ObjectCode { get; set; }
        public string? DocSubType { get; set; }
        public DocumentNumberingSeriesFindEntity ReturnValue()
        {
            return new DocumentNumberingSeriesFindEntity
            {
                ObjectCode = ObjectCode,
                DocSubType = DocSubType
            };
        }
    }
}

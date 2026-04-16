namespace Net.Business.DTO.SAPBusinessOne
{
    public class StockTransfers1UpdateRequestDto
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }
        public string FromWhsCod { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
    }
}

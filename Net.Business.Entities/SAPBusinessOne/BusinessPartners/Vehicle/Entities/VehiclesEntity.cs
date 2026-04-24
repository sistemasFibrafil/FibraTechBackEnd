namespace Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicle.Entities
{
    public class VehiclesEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; }
        public string? U_BPP_VEPL { get; set; }
        public string? U_BPP_VEMA { get; set; }
        public string? U_BPP_VEMO { get; set; }
        public string? U_BPP_VEAN { get; set; }
        public string? U_BPP_VECO { get; set; }
        public string? U_BPP_VESE { get; set; }
        public decimal? U_BPP_VEPM { get; set; }
        public string? U_FIB_COTR { get; set; }
    }
}

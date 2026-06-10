namespace Net.Business.Entities.SAPBusinessOne.Inventory.Items.Update
{
    public class ItemsUpdateMassiveEntity
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public short ItmsGrpCod { get; set; }
        public string? U_BPP_TIPEXIST { get; set; }
        public string? U_BPP_TIPUNMED { get; set; }
        public string? U_S_PartAranc1 { get; set; }
        public string? U_S_PartAranc2 { get; set; }
        public string? U_FIB_ECU { get; set; }
        public string? U_S_CCosto { get; set; }
        public decimal U_FIB_PESO { get; set; }
        public string? U_FIB_SGRUP { get; set; }
        public string? U_FIB_SGRUPO2 { get; set; }
        public string? U_FIB_LINNEG { get; set; }
        public int U_UsrUpdate { get; set; }
    }
}

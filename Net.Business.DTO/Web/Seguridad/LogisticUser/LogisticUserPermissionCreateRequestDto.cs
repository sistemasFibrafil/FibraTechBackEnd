namespace Net.Business.DTO.Web
{
    public class LogisticUserPermissionCreateRequestDto
    {
        public int IdLogisticUserPermission { get; set; }
        public int IdLogisticUser { get; set; }
        public string ObjectType { get; set; }
        public string WhsCode { get; set; }
        public string ToWhsCode { get; set; }
        public bool Blocked { get; set; }
    }
}

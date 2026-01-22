namespace Net.Business.Entities.Web
{
    public class LogisticUserPermissionEntity
    {
        public int IdLogisticUserPermission { get; set; }
        public int IdLogisticUser { get; set; }
        public string ObjectType { get; set; }
        public string WhsCode { get; set; }
        public string ToWhsCode { get; set; }
        public bool Blocked { get; set; }


        // navegación N:1
        public LogisticUserEntity LogisticUser { get; set; }
    }
}

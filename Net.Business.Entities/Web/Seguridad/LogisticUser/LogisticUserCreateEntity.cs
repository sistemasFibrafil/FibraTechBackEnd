using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class LogisticUserCreateEntity
    {
        public int IdLogisticUser { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdLocation { get; set; }
        public bool SuperUser { get; set; }
        public bool Blocked { get; set; }
        public ICollection<LogisticUserPermissionCreateEntity> Permissions { get; set; } = new List<LogisticUserPermissionCreateEntity>();
    }
}

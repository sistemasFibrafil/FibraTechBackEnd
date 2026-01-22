using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class LogisticUserQueryEntity
    {
        public int? IdLogisticUser { get; set; } = 0;
        public int? IdUsuario { get; set; } = 0;
        public int? IdLocation { get; set; } = 0;
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public bool SuperUser { get; set; }
        public bool Blocked { get; set; }

        public ICollection<LogisticUserPermissionEntity> Permissions { get; set; } = new List<LogisticUserPermissionEntity>();
    }
}

using System.Collections.Generic;

namespace Net.Business.Entities.Web
{
    public class LogisticUserEntity
    {
        public int IdLogisticUser { get; set; }
        // FK hacia Usuario: nullable para que la relación pueda ser opcional (LEFT JOIN)
        public int? IdUsuario { get; set; }
        public int? IdLocation { get; set; }
        public bool? SuperUser { get; set; }
        public bool Blocked { get; set; }


        // Navegación inversa 1:1
        public UsuarioEntity Usuario { get; set; }

        // Navegación 1:N a permisos
        public ICollection<LogisticUserPermissionEntity> Permissions { get; set; }
    }
}

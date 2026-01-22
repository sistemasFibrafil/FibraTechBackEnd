using System.Linq;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Business.DTO.Web
{
    public class LogisticUserCreateRequestDto
    {
        public int IdLogisticUser { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdLocation { get; set; }
        public bool SuperUser { get; set; }
        public bool Blocked { get; set; }
        public ICollection<LogisticUserPermissionCreateRequestDto> Permissions { get; set; } = new List<LogisticUserPermissionCreateRequestDto>();

        public LogisticUserCreateEntity ReturnValue()
        {
            var permissions = Permissions.Select(permissions => new LogisticUserPermissionCreateEntity
            {
                IdLogisticUserPermission = permissions.IdLogisticUserPermission,
                IdLogisticUser = permissions.IdLogisticUser,
                ObjectType = permissions.ObjectType,
                WhsCode = permissions.WhsCode,
                ToWhsCode = permissions.ToWhsCode,
                Blocked = permissions.Blocked
            }).ToList();

            return new LogisticUserCreateEntity
            {
                IdLogisticUser = this.IdLogisticUser,
                IdUsuario = this.IdUsuario,
                IdLocation = this.IdLocation,
                SuperUser = this.SuperUser,
                Blocked = this.Blocked,
                Permissions = permissions
            };
        }
    }
}

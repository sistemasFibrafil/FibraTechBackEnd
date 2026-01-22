using System.Data;
using Net.Connection.Attributes;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class PerilEntity : BaseEntity
    {
        /// <summary>
        /// IdPerfil
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdPerfil { get; set; }
        /// <summary>
        /// DescripcionPerfil
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionPerfil { get; set; }
        /// <summary>
        /// FlgActivo
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? Activo { get; set; }
        public bool? Eliminado { get; set; }


        // Navegación 1:N → Un perfil puede tener muchos usuarios
        public ICollection<UsuarioEntity> Usuarios { get; set; }
    }
}

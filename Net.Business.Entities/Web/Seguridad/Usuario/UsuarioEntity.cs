using System;
using System.Data;
using Net.Connection.Attributes;
namespace Net.Business.Entities.Web
{
    public class UsuarioEntity
    {
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int IdUsuario { get; set; }
        public int? IdPerfil { get; set; }
        public int? IdUserSap { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NroDocumento { get; set; }
        public string NroTelefono { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Email { get; set; }
        public string Imagen { get; set; }
        public string Firma { get; set; }
        public bool? ThemeDark { get; set; }
        public string ThemeColor { get; set; }
        public string TypeMenu { get; set; }
        public bool? Activo { get; set; }
        public bool? Eliminado { get; set; }


        // Navegación N:1
        public PerilEntity Perfil { get; set; }
        public LogisticUserEntity LogisticUser { get; set; }
    }

    public class UsuarioTablaEntity
    {
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string Tabla { get; set; }
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string CodigoTabla { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool FlgActivo { get; set; }
    }

    public class UsuarioNegocioEntity
    {
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int GroupCode { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool FlgActivo { get; set; }
    }

    public class UsuarioAlmacenEntity
    {
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string WarehouseCode { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool WarehouseDeseDefault { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool FlgActivo { get; set; }
    }

    public class UsuarioRecuperarPasswordEntity
    {
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string Sociedad { get; set; }
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string Usuario { get; set; }
    }

    public class UsuarioTokenEntity : BaseEntity
    {
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string Token { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime? FecExpToken { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int IdUsuario { get; set; }
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string Usuario { get; set; }
    }

}

using System;
using System.Data;
using Net.Connection.Attributes;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class UsuarioEntity : BaseEntity
    {
        /// <summary>
        /// IdUsuario
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdUsuario { get; set; }
        /// <summary>
        /// IdPersona
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdPersona { get; set; }
        /// <summary>
        /// IdPerfil
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdPerfil { get; set; }
        /// <summary>
        /// CodSede
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? CodSede { get; set; }
        /// <summary>
        /// IsNotRestAlmacen
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? IsNotRestAlmacen { get; set; }
        /// <summary>
        /// DescripcionPerfil
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionPerfil { get; set; }
        /// <summary>
        /// Usuario
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string Usuario { get; set; }
        /// <summary>
        /// Clave Texto Origen
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string ClaveOrigen { get; set; }
        /// <summary>
        /// Clave Encriptada
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string Clave { get; set; }
        /// <summary>
        /// Email del usuario
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string Email { get; set; }
        /// <summary>
        /// Imagen
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string Imagen { get; set; }
        /// <summary>
        /// Imagen
        /// </summary>
        [DBParameter(SqlDbType.Text, 0, ActionType.Everything)]
        public string Firma { get; set; }
        /// <summary>
        /// ThemeDark
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? ThemeDark { get; set; }
        /// <summary>
        /// ThemeColor
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string ThemeColor { get; set; }
        /// <summary>
        /// TypeMenu
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 20, ActionType.Everything)]
        public string TypeMenu { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string Nombre { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 10, ActionType.Everything)]
        public string CodCentroCosto { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DesCentroCosto { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? CodVendedorSAP { get; set; }
        [DBParameter(SqlDbType.VarChar, 8, ActionType.Everything)]
        public string WarehouseDefault { get; set; }
        public List<UsuarioTablaEntity> ListUsuarioTabla { get; set; }
        public List<UsuarioNegocioEntity> ListUsuarioNegocio { get; set; }
        public List<UsuarioAlmacenEntity> ListUsuarioAlmacen { get; set; }
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

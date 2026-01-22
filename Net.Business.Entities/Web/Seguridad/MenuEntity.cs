using System.Data;
using Net.Connection.Attributes;
using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class MenuEntity : BaseEntity
    {
        /// <summary>
        /// IdMenu
        /// </summary>
        [DBParameter(SqlDbType.Int, ActionType.Everything, true)]
        public int? IdMenu { get; set; }
        /// <summary>
        /// DescripcionTitulo
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string DescripcionTitulo { get; set; }
        /// <summary>
        /// Icono
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string Icono { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 500, ActionType.Everything)]
        public string Url { get; set; }
        /// <summary>
        /// NroNivel
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? NroNivel { get; set; }
        /// <summary>
        /// DescripcionTitulo
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgActivo { get; set; }
        /// <summary>
        /// IdMenuPadre
        /// </summary>
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int? IdMenuPadre { get; set; }
        /// <summary>
        /// FlgChildren
        /// </summary>
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool? FlgChildren { get; set; }
        /// <summary>
        /// NombreFormulario
        /// </summary>
        [DBParameter(SqlDbType.NVarChar, 100, ActionType.Everything)]
        public string NombreFormulario { get; set; }
        /// <summary>
        /// ListaOpciones
        /// </summary>
        public IEnumerable<OpcionEntity> ListaOpciones { get; set; }
    }
}

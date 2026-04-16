using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class UsuarioDatosEntity
    {
        public int? IdUsuario { get; set; }
        public int? IdPerfil { get; set; }
        public int? IdUserSap { get; set; }
        public string? UserSap { get; set; }
        public int? IdLocation { get; set; }
        public bool SuperUser { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public string? Imagen { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? CompnyName { get; set; }
        public string? CompnyAddr { get; set; }
        public string? PrintHeadr { get; set; }
        public string? TaxIdNum { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? MainCurncy { get; set; }
        public string? SysCurrncy { get; set; }
        public string? DfltWhs { get; set; }
        public string? AttachPath { get; set; }
        public string? WhsCodeSpaPar { get; set; }
        public int CodGrpSuppNat { get; set; }
        public int CodGrpSuppFor { get; set; }
        public int CodGrpCustNat { get; set; }
        public int CodGrpCustFor { get; set; }
        public List<MenuEntity> ListaAccesoMenu { get; set; } = new List<MenuEntity>();
    }
}

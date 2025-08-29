using System.Collections.Generic;
namespace Net.Business.Entities.Web
{
    public class UsuarioDatosEntity
    {
        public int? IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Imagen { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public int? IdPersona { get; set; }
        public int? IdPerfil { get; set; }
        public int? CodSede { get; set; }
        public bool? IsNotRestAlmacen { get; set; }
        public int? CodVendedorSAP { get; set; }
        public string WarehouseDefault { get; set; }
        public string CompnyName { get; set; }
        public string CompnyAddr { get; set; }
        public string PrintHeadr { get; set; }
        public string TaxIdNum { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string MainCurncy { get; set; }
        public string SysCurrncy { get; set; }
        public List<MenuEntity> ListaAccesoMenu { get; set; }
    }
}

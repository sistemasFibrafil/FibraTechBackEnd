using System;
namespace Net.Business.Entities.Sap
{
    public class OrdenFabricacionSapEntity
    {
        public int IdProducion { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodeBar { get; set; }
        public string CantidadPlanificada { get; set; }
        public string UnidadMedida { get; set; }
        public int BultoProcesado { get; set; }
        public decimal PesoProcesado { get; set; }
        public DateTime Fecha { get; set; }
        public string Maquina { get; set; }
    }

    public class OrdenFabricacionGeneralSapBySedeEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime FechaOrdenFabricacion { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaSistema { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public string NormaReparto { get; set; }
        public string ItemProd { get; set; }
        public string DscItemProd { get; set; }
        public string Almacen { get; set; }
        public string UnidadMedida { get; set; }
        public decimal QProd { get; set; }
        public decimal PesoProd { get; set; }
        public string Grupo { get; set; }
        public string SubGrupo { get; set; }
        public string SubGrupo2 { get; set; }
        public string ItemBase { get; set; }
        public string DscItemBase { get; set; }
        public decimal QBase { get; set; }
        public decimal Planificado { get; set; }
        public decimal IssuedQty { get; set; }
        public string UnidadMedidaInventario { get; set; }
        public decimal Precio { get; set; }
        public string WareHouse { get; set; }
        public decimal CantMillar { get; set; }
        public string Situacion { get; set; }
        public string DestinoProd { get; set; }
        public string Maquina { get; set; }
        public string Usuario { get; set; }
        public string CentoCosto { get; set; }
        public string Comentarios { get; set; }
        public string Sede { get; set; }
    }
}

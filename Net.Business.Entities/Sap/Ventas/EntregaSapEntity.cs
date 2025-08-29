using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class EntregaSapEntity
    {
        public int IdEntregaVenta { get; set; }
        public int DocEntry { get; set; }
        public int Series { get; set; }
        public int DocNum { get; set; }
        public string DocStatus { get; set; }
        public string SerieSunat { get; set; }
        public string NumeroSunat { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }

        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string LicTradNum { get; set; }
        public string DocCur { get; set; }
        public double DocRate { get; set; }

        public string ShipToCode { get; set; }
        public string Address2 { get; set; }
        public string PayToCode { get; set; }
        public string Address { get; set; }
        public string CodMotTraslado { get; set; }
        public string OtrMotTraslado { get; set; }

        public int CodCondicionPago { get; set; }

        public string RucDesInternacional { get; set; }
        public string DesGuiaInternacional { get; set; }
        public string DirDesInternacional { get; set; }
        public string NumContenedor { get; set; }
        public string NumPrecinto01 { get; set; }
        public string NumPrecinto02 { get; set; }
        public string NumPrecinto03 { get; set; }
        public string NumPrecinto04 { get; set; }

        //Transportista
        public string CodTipTransporte { get; set; }
        //Transportista 1
        public bool ManTransportista1 { get; set; }
        public string CodTransportista1 { get; set; }
        public string CodTipDocIdeTransportista1 { get; set; }
        public string RucTransportista1 { get; set; }
        public string NomTransportista1 { get; set; }
        public string NumPlaca1 { get; set; }

        public string CodTipDocIdeConductor1 { get; set; }
        public string NumDocIdeConductor1 { get; set; }
        public string DenConductor1 { get; set; }
        public string NomConductor1 { get; set; }
        public string ApeConductor1 { get; set; }
        public string LicConductor1 { get; set; }
        //Transportista 2
        public bool ManTransportista2 { get; set; }
        public string CodTransportista2 { get; set; }
        public string RucTransportista2 { get; set; }
        public string NomTransportista2 { get; set; }
        public string DirTransportista2 { get; set; }

        public int SlpCode { get; set; }
        public double TotalBulto { get; set; }
        public double TotalKilo { get; set; }
        public string Comments { get; set; }

        public double VatSum { get; set; }
        public double VatSumFC { get; set; }
        public double VatSumy { get; set; }

        public double DocTotal { get; set; }
        public double DocTotalFC { get; set; }
        public double DocTotalSy { get; set; }
        public int IdUsuario { get; set; }

        public List<EntregaVentaDetalleSapEntity> Item { get; set; } = new List<EntregaVentaDetalleSapEntity>();
    }

    public class EntregaVentaDetalleSapEntity
    {
        public int IdEntregaVentaItem { get; set; }
        public int IdEntregaVenta { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; }
        public int IdBase { get; set; }
        public int IdBaseItem { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public double Quantity { get; set; }
        public double Peso { get; set; }
        public string TaxCode { get; set; }
        public string AcctCode { get; set; }
        public string Currency { get; set; }
        public double DiscPrcnt { get; set; }
        public double Price { get; set; }
        public double LineTotal { get; set; }
        public double TotalFrgn { get; set; }
        public double TotalSumSy { get; set; }
    }

    public class EntregaSapByFechaEntity
    {
        public DateTime FechaEmision { get; set; }
        public string Tipo { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }

        public string ClienteTipoDocumento { get; set; }
        public string ClienteNumeroDocumento { get; set; }
        public string ClienteDenominacion { get; set; }

        public string Detalle { get; set; }
        public decimal PesoBruto { get; set; }
        public string PesoUnidadMedida { get; set; }
        public DateTime FechaTraslado { get; set; }

        public string TransportistaDocumentoTipo { get; set; }
        public string TransportistaDocumentoNumero { get; set; }
        public string TransportistaDenominacion { get; set; }
        public string TransportistaPlacaNumero { get; set; }

        public string ConductorDocumentoTipo { get; set; }
        public string ConductorDocumentoNumero { get; set; }
        public string ConductorNombre { get; set; }
        public string ConductorApellidos { get; set; }
        public string ConductorLicenciaNumero { get; set; }

        public string PuntoPartidaUbigeo { get; set; }
        public string PuntoPartidaDireccion { get; set; }
        public string PuntoLlegadaUbigeo { get; set; }
        public string PuntoLlegadaDireccion { get; set; }

        public string Observaciones { get; set; }
        public string EstadoSunat { get; set; }
    }
}

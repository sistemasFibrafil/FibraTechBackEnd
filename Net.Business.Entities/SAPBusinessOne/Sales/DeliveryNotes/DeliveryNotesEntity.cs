using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class DeliveryNotesEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public string DocType { get; set; } = string.Empty;
        public string CANCELED { get; set; } = string.Empty;
        public string DocStatus { get; set; } = string.Empty;

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public short DocTime { get; set; }
        public short UserSign { get; set; }

        public string? U_FIB_FromPkg { get; set; }

        public string? U_BPP_MDTD { get; set; }
        public string? U_BPP_MDSD { get; set; }
        public string? U_BPP_MDCD { get; set; }



        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string CardCode { get; set; } = string.Empty;
        public string? CardName { get; set; }
        public short GroupCode { get; set; }
        public int? CntctCode { get; set; }
        public string? NumAtCard { get; set; }
        public string? DocCur { get; set; }
        public decimal DocRate { get; set; }



        /// <summary>
        /// LOGISTICA
        /// </summary>
        public string? PayToCode { get; set; }
        public string? Address { get; set; }
        public string? ShipToCode { get; set; }
        public string? Address2 { get; set; }



        /// <summary>
        /// FINANZAS
        /// </summary>
        public short GroupNum { get; set; }



        /// <summary>
        /// AGENCIA
        /// </summary>
        public string? U_BPP_MDCT { get; set; }
        public string? U_BPP_MDRT { get; set; }
        public string? U_BPP_MDNT { get; set; }
        public string? U_FIB_CODT { get; set; }
        public string? U_BPP_MDDT { get; set; }



        /// <summary>
        /// TRANSPORTISTA
        /// </summary>
        public string? U_FIB_TIP_TRANS { get; set; }
        public string? U_FIB_COD_TRA { get; set; }
        public string? U_FIB_TIPDOC_TRA { get; set; }
        public string? U_FIB_RUC_TRANS2 { get; set; }
        public string? U_FIB_TRANS2 { get; set; }
        public string? U_BPP_MDVC { get; set; }
        public string? U_FIB_TIPDOC_COND { get; set; }
        public string? U_FIB_NUMDOC_COD { get; set; }
        public string? U_FIB_NOM_COND { get; set; }
        public string? U_FIB_APE_COND { get; set; }
        public string? U_BPP_MDFN { get; set; }
        public string? U_BPP_MDFC { get; set; }



        /// <summary>
        /// EXPORTACION
        /// </summary>
        public string? U_RUCDestInter { get; set; }
        public string? U_DestGuiaInter { get; set; }
        public string? U_DireccDestInter { get; set; }
        public string? U_STR_NCONTENEDOR { get; set; }
        public string? U_STR_NPRESCINTO { get; set; }
        public string? U_FIB_NPRESCINTO2 { get; set; }
        public string? U_FIB_NPRESCINTO3 { get; set; }
        public string? U_FIB_NPRESCINTO4 { get; set; }



        /// <summary>
        /// OTROS
        /// </summary>
        public string? U_STR_TVENTA { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDOM { get; set; }



        // <summary>
        /// SALES EMPLOYEE
        /// </summary>
        public int SlpCode { get; set; }
        public decimal? U_FIB_NBULTOS { get; set; }
        public decimal? U_FIB_KG { get; set; }
        public string? U_NroOrden { get; set; }
        public string? U_OrdenCompra { get; set; }
        public string? Comments { get; set; }


        /// <summary>
        /// TOTALES
        /// </summary>
        public decimal SubTotal { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal DiscSum { get; set; }
        public decimal DiscSumSy { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumSy { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalSy { get; set; }


        // 🔗 1 → N (ODLN → OUSR)
        public UsersEntity? Users { get; set; } = null;


        // 🔗 1 → N (ODLN → OCRD)
        public BusinessPartnersEntity? BusinessPartners { get; set; } = null;


        // 🔗 1 → N (ODLN → OCRN)
        public CurrencyCodesEntity? CurrencyCodes { get; set; } = null;


        // 🔗 1 → N (ODLN → OSLP)
        public SalesPersonsEntity? SalesPersons { get; set; } = null;


        // 🔗 1 → N (ODLN → OCTG)
        public PaymentTermsTypesEntity? PaymentTermsTypes { get; set; } = null;


        // 🔗 1 → N (ODLN → DLN1)
        public List<DeliveryNotes1Entity> Lines { get; set; } = new List<DeliveryNotes1Entity>();
    }

    public class DeliveryNotes1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }

        public string LineStatus { get; set; } = string.Empty;
        public string ObjType { get; set; } = string.Empty;
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }

        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? AcctCode { get; set; }
        public string? WhsCode { get; set; }

        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public string? Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string? TaxCode { get; set; }
        public decimal? VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumSy { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TotalSumSy { get; set; }

        public decimal? U_FIB_NBulto { get; set; }
        public string? U_FIB_FromPkg { get; set; }
        public decimal? U_FIB_PesoKg { get; set; }
        public string? U_tipoOpT12 { get; set; }


        // 🔗 N → 1 (DLN1 → OITM)
        public ItemsEntity? Item { get; set; } = null;


        // 🔗 N → 1 (DLN1 → OACT)
        public ChartOfAccountsEntity? ChartOfAccounts { get; set; } = null;


        // 🔗 N → 1 (DLN1 → TipoOperacion)
        public OperationTypeEntity? OperationType { get; set; } = null;
    }

    public class DeliveryNotesByFechaEntity
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

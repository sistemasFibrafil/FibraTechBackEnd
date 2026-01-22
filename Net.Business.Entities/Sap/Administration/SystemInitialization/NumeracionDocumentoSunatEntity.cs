namespace Net.Business.Entities.Sap
{
    public class NumeracionDocumentoSunatEntity
    {
        public string Code { get; set; }
        public string U_BPP_NDTD { get; set; }
        public string U_BPP_NDSD { get; set; }
        public string U_BPP_NDCD { get; set; }
        public int U_FIB_SEDE { get; set; }

        // Tipo de Documento de Entrega TDED = Documento de Entrega
        public string U_FIB_TDED { get; set; }

        // Tipo de Documento de Transferencia DTD = Documento de Transferencia
        public string U_FIB_TDTD { get; set; }

        // Serie de Documento de Entrega por defecto
        public string U_FIB_SDED { get; set; }

        // Serie de Documento de Transferencia por defecto
        public string U_FIB_SDTD { get; set; }
    }
}

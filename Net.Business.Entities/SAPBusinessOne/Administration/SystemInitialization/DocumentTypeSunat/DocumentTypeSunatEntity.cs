namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentTypeSunatEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? U_BPP_TDTD { get; set; }
        public string? U_BPP_TDDD { get; set; }


        /// <summary>
        /// Tipo de Documento de Entrega: Puede ser Y o N
        /// </summary>
        public string? U_FIB_ENTR { get; set; }
        /// <summary>
        /// Tipo de Documento de Entrega por defecto: Puede ser Y o N
        /// </summary>
        public string? U_FIB_ENDF { get; set; }
        /// <summary>
        /// Tipo de Documento de Entrega anulada: Puede ser Y o N
        /// </summary>
        public string? U_FIB_ENAN { get; set; }


        /// <summary>
        /// Tipo de Documento de Factura de Venta: Puede ser Y o N
        /// </summary>
        public string? U_FIB_FAVE { get; set; }
        /// <summary>
        /// Tipo de Documento de Factura de Venta por defecto: Puede ser Y o N
        /// </summary>
        public string? U_FIB_FVDF { get; set; }
        /// <summary>
        /// Tipo de Documento de Factura de Venta anulacion: Puede ser Y o N
        /// </summary>
        public string? U_FIB_FVAN { get; set; }


        /// <summary>
        /// Tipo de Documento de Transferencia: Puede ser Y o N
        /// </summary>
        public string? U_FIB_TRAN { get; set; }
    }
}

using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class FacturacionElectronicaSapEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public string CodStatusSunat { get; set; }
        public string NomStatusSunat { get; set; }

        public string NumeroDocumento { get; set; }
        public string ClienteNumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteDenominacion { get; set; }

        public DateTime FechaEmision { get; set; }
        public DateTime FechaEntrega { get; set; }
    }

    public class Invoice
    {
        public Invoice()
        {
            items = new List<Items>();
        }

        public string operacion { get; set; }
        public string tipo_de_comprobante { get; set; }
        public string serie { get; set; }
        public string numero { get; set; }
        public string sunat_transaction { get; set; }
        public string cliente_tipo_de_documento { get; set; }
        public string cliente_numero_de_documento { get; set; }
        public string cliente_denominacion { get; set; }
        public string cliente_direccion { get; set; }
        public string cliente_email { get; set; }
        public string cliente_email_1 { get; set; }
        public string cliente_email_2 { get; set; }
        public string fecha_de_emision { get; set; }
        public string fecha_de_vencimiento { get; set; }
        public string fecha_de_inicio_de_traslado { get; set; }
        public string moneda { get; set; }
        public dynamic tipo_de_cambio { get; set; }
        public double porcentaje_de_igv { get; set; }
        public dynamic descuento_global { get; set; }
        public dynamic total_descuento { get; set; }
        public dynamic total_anticipo { get; set; }
        public dynamic total_gravada { get; set; }
        public dynamic total_inafecta { get; set; }
        public dynamic total_exonerada { get; set; }
        public double total_igv { get; set; }
        public dynamic total_gratuita { get; set; }
        public dynamic total_otros_cargos { get; set; }
        public double total { get; set; }
        public dynamic percepcion_tipo { get; set; }
        public dynamic percepcion_base_imponible { get; set; }
        public dynamic total_percepcion { get; set; }
        public dynamic total_incluido_percepcion { get; set; }
        public bool detraccion { get; set; }
        public string observaciones { get; set; }

        public string motivo_de_traslado { get; set; }
        public string motivo_de_traslado_otros_descripcion { get; set; }
        public decimal peso_bruto_total { get; set; }
        public string peso_bruto_unidad_de_medida { get; set; }
        public decimal numero_de_bultos { get; set; }
        public string tipo_de_transporte { get; set; }

        public string transportista_documento_tipo { get; set; }
        public string transportista_documento_numero { get; set; }
        public string transportista_denominacion { get; set; }
        public string transportista_placa_numero { get; set; }

        public string conductor_documento_tipo { get; set; }
        public string conductor_documento_numero { get; set; }
        public string conductor_denominacion { get; set; }
        public string conductor_nombre { get; set; }
        public string conductor_apellidos { get; set; }
        public string conductor_numero_licencia { get; set; }

        public string punto_de_partida_ubigeo { get; set; }
        public string punto_de_partida_direccion { get; set; }
        public string punto_de_partida_codigo_establecimiento_sunat { get; set; }

        public string punto_de_llegada_ubigeo { get; set; }
        public string punto_de_llegada_direccion { get; set; }
        public string punto_de_llegada_codigo_establecimiento_sunat { get; set; }

        public dynamic documento_que_se_modifica_tipo { get; set; }
        public string documento_que_se_modifica_serie { get; set; }
        public dynamic documento_que_se_modifica_numero { get; set; }
        public dynamic tipo_de_nota_de_credito { get; set; }
        public dynamic tipo_de_nota_de_debito { get; set; }
        public bool enviar_automaticamente_a_la_sunat { get; set; }
        public bool enviar_automaticamente_al_cliente { get; set; }
        public string codigo_unico { get; set; }
        public string condiciones_de_pago { get; set; }
        public string medio_de_pago { get; set; }
        public string placa_vehiculo { get; set; }
        public string orden_compra_servicio { get; set; }
        public string tabla_personalizada_codigo { get; set; }
        public string formato_de_pdf { get; set; }
        public string documento_relacionado_codigo { get; set; }
        public List<Items> items { get; set; }
        public List<Guias> guias { get; set; }
    }
    public class Items
    {
        public string unidad_de_medida { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal valor_unitario { get; set; }
        public decimal precio_unitario { get; set; }
        public dynamic descuento { get; set; }
        public decimal subtotal { get; set; }
        public int tipo_de_igv { get; set; }
        public decimal igv { get; set; }
        public decimal total { get; set; }
        public bool anticipo_regularizacion { get; set; }
        public dynamic anticipo_comprobante_serie { get; set; }
        public dynamic anticipo_comprobante_numero { get; set; }
    }
    public class Facturas
    {
        public int DocEntry { get; set; }
        public string ObjType { get; set; }
        public string Qry { get; set; }
        public string Tsq { get; set; }
    }
    public class Guias
    {
        public int guia_tipo { get; set; }
        public string guia_serie_numero { get; set; }
    }
    public class Respuesta
    {
        public string errors { get; set; }
        public int tipo { get; set; }
        public string serie { get; set; }
        public int numero { get; set; }
        public string url { get; set; }
        public bool aceptada_por_sunat { get; set; }
        public string sunat_description { get; set; }
        public string sunat_note { get; set; }
        public string sunat_responsecode { get; set; }
        public string sunat_soap_error { get; set; }
        public string pdf_zip_base64 { get; set; }
        public string xml_zip_base64 { get; set; }
        public string cdr_zip_base64 { get; set; }
        public string cadena_para_codigo_qr { get; set; }
        public string codigo_hash { get; set; }
        public string codigo_de_barras { get; set; }
    }
}

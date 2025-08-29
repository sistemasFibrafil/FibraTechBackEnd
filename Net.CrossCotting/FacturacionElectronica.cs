using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Net.Business.Entities.Sap;

namespace Net.CrossCotting
{
    public static class FacturacionElectronica
    {
        /// # RUTA para enviar documentos
        public const string ruta = "https://api.nubefact.com/api/v1/9469d6b7-da70-4397-9db1-cba2a0ad8398";
        /// # TOKEN para enviar documentos
        public const string token = "73fdf67e07fd4f65bdd2439f28f17fc9efcac960c0e04545854876200db10f61";


        public static Respuesta GetResponse(string tsq)
        {
            byte[] bytes = Encoding.Default.GetBytes(tsq);
            string json_en_utf_8 = Encoding.UTF8.GetString(bytes);

            // Envio de XML a NUBEFACT
            string respuesta = FacturacionElectronica.SendJson(json_en_utf_8);
            dynamic r = JsonConvert.DeserializeObject<Respuesta>(respuesta);
            string r2 = JsonConvert.SerializeObject(r, Formatting.Indented);
            dynamic json_r_in = JsonConvert.DeserializeObject<Respuesta>(r2);

            // Obtenemos la respuesta de NUBEFACT
            return JsonConvert.DeserializeObject<Respuesta>(respuesta);
        }
        public static string SendJson(string json)
        {
            try
            {
                using (var client = new WebClient())
                {
                    /// ESPECIFICAMOS EL TIPO DE DOCUMENTO EN EL ENCABEZADO
                    client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";

                    /// ASI COMO EL TOKEN UNICO
                    client.Headers[HttpRequestHeader.Authorization] = "Token token=" + token;

                    /// OBTENEMOS LA RESPUESTA
                    string respuesta = client.UploadString(ruta, "POST", json);

                    /// Y LA 'RETORNAMOS'
                    return respuesta;
                }
            }
            catch (WebException ex)
            {
                /// EN CASO EXISTA ALGUN ERROR, LO TOMAMOS
                var respuesta = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                /// Y LO 'RETORNAMOS'
                return respuesta;
            }
        }
    }
}

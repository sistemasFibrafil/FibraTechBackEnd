using System.Collections.Generic;

namespace Net.CrossCotting
{
    public static class ResponseHelper
    {
        // ✅ SUCCESS CON DATA
        public static ResultadoTransaccionResponse<T> Success<T>(T data, string message = "OK")
        {
            return new ResultadoTransaccionResponse<T>
            {
                ResultadoCodigo = 0,
                ResultadoDescripcion = message,
                data = data
            };
        }

        // ✅ SUCCESS SIN DATA (🔥 RECOMENDADO)
        public static ResultadoTransaccionResponse<T> Success<T>(string message = "OK")
        {
            return new ResultadoTransaccionResponse<T>
            {
                ResultadoCodigo = 0,
                ResultadoDescripcion = message
            };
        }

        // ✅ SUCCESS CON LISTA
        public static ResultadoTransaccionResponse<T> SuccessList<T>(IEnumerable<T> dataList, string message = "OK")
        {
            return new ResultadoTransaccionResponse<T>
            {
                ResultadoCodigo = 0,
                ResultadoDescripcion = message,
                dataList = dataList
            };
        }

        // ❌ ERROR
        public static ResultadoTransaccionResponse<T> Error<T>(string message, int codigo = -1)
        {
            return new ResultadoTransaccionResponse<T>
            {
                ResultadoCodigo = codigo,
                ResultadoDescripcion = message
            };
        }

        // ⚠️ WARNING (opcional)
        public static ResultadoTransaccionResponse<T> Warning<T>(string message)
        {
            return new ResultadoTransaccionResponse<T>
            {
                ResultadoCodigo = 1,
                ResultadoDescripcion = message
            };
        }

        // 🔁 MAP GENERIC (🔥 para tu caso del repo)
        public static ResultadoTransaccionResponse<object> From<T>(ResultadoTransaccionResponse<T> source)
        {
            return new ResultadoTransaccionResponse<object>
            {
                ResultadoCodigo = source.ResultadoCodigo,
                ResultadoDescripcion = source.ResultadoDescripcion,
                data = source.data,
                dataList = source.dataList as IEnumerable<object>
            };
        }
    }
}

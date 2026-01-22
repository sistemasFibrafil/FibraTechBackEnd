using System;
using Net.Business.Entities;
using Microsoft.Extensions.Configuration;
namespace Net.CrossCotting
{
    public static class Utilidades
    {
        private static string _cnx;
        private static string _entorno;
        private static string _source;
        private static string _catalog;
        private static string _user;
        private static string _password;

        private static string _server;
        private static string _licenseServer;
        private static string _dbUserName;
        private static string _dbPassword;
        private static string _dbServerType;

        private static string _companyDB;
        private static string _userName;


        public static string GetCon(IConfiguration configuration, string entorno)
        {
            _cnx = configuration.GetConnectionString("cnnSql");
            _entorno = configuration[entorno];

            //_source = configuration[string.Format("{0}:Source", _entorno)];
            //_catalog = configuration[string.Format("{0}:Catalog", _entorno)];
            //_user = configuration[string.Format("{0}:User", _entorno)];
            //_password = configuration[string.Format("{0}:Password", _entorno)];

            //_source = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:Source", _entorno)]);
            //_catalog = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:Catalog", _entorno)]);
            //_user = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:User", _entorno)]);
            //_password = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:Password", _entorno)]);

            _source = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:Source", _entorno)]);
            _catalog = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:Catalog", _entorno)]);
            _user = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:User", _entorno)]);
            _password = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:Password", _entorno)]);

            _cnx = _cnx.Replace("{Source}", _source).Replace("{Catalog}", _catalog).Replace("{User}", _user).Replace("{Password}", _password);

            return _cnx;
        }

        public static ConnectionSapEntity GetConSap(IConfiguration configuration, string entorno)
        {
            _entorno = configuration[entorno];

            //_server = configuration[string.Format("{0}:Server", _entorno)];
            //_licenseServer = configuration[string.Format("{0}:LicenseServer", _entorno)];
            //_dbUserName = configuration[string.Format("{0}:DbUserName", _entorno)];
            //_dbPassword = configuration[string.Format("{0}:DbPassword", _entorno)];
            //_dbServerType = configuration[string.Format("{0}:DbServerType", _entorno)];

            //_companyDB = configuration[string.Format("{0}:CompanyDB", _entorno)];
            //_userName = configuration[string.Format("{0}:UserName", _entorno)];
            //_password = configuration[string.Format("{0}:Password", _entorno)];

            //_server = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:Server", _entorno)]);
            //_licenseServer = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:LicenseServer", _entorno)]);
            //_dbUserName = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:DbUserName", _entorno)]);
            //_dbPassword = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:DbPassword", _entorno)]);
            //_dbServerType = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:DbServerType", _entorno)]);

            //_companyDB = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:CompanyDB", _entorno)]);
            //_userName = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:UserName", _entorno)]);
            //_password = EncriptaHelper.EncryptStringAES(configuration[string.Format("{0}:Password", _entorno)]);

            _server = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:Server", _entorno)]);
            _licenseServer = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:LicenseServer", _entorno)]);
            _dbUserName = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:DbUserName", _entorno)]);
            _dbPassword = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:DbPassword", _entorno)]);
            _dbServerType = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:DbServerType", _entorno)]);

            _companyDB = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:CompanyDB", _entorno)]);
            _userName = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:UserName", _entorno)]);
            _password = EncriptaHelper.DecryptStringAES(configuration[string.Format("{0}:Password", _entorno)]);

            var _cnx = new ConnectionSapEntity()
            {
                Server = _server,
                LicenseServer = _licenseServer,
                DbUserName = _dbUserName,
                DbPassword = _dbPassword,
                DbServerType = _dbServerType,

                CompanyDB = _companyDB,
                UserName = _userName,
                Password = _password,
            };

            return _cnx;
        }

        public static DateTime DateTimeEmpty()
        {
            return new DateTime(1, 1, 1, 0, 0, 0);
        }

        public static DateTime GetFechaHoraInicioActual(DateTime? fecha)
        {
            DateTime? data;

            data = fecha;

            if (fecha == null || fecha.Equals(DateTimeEmpty()))
            {
                data = new DateTime(DateTime.Now.Year, 1, DateTime.Now.Day, 0, 0, 0);
            }
            else
            {
                data = new DateTime(((DateTime)fecha).Year, ((DateTime)fecha).Month, ((DateTime)fecha).Day, 0, 0, 0);
            }

            return (DateTime)data;
        }

        public static DateTime GetFechaHoraFinActual(DateTime? fecha)
        {
            DateTime? data;

            data = fecha;

            if (fecha == null || fecha.Equals(DateTimeEmpty()))
            {
                data = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            }
            else
            {
                data = new DateTime(((DateTime)fecha).Year, ((DateTime)fecha).Month, ((DateTime)fecha).Day, 23, 59, 59);
            }

            return (DateTime)data;
        }

        public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (T)inst?.Invoke(obj, null);
        }

        public static string MensajeError(string mensaje)
        {
            int indexDescripcion = mensaje.IndexOf("*");

            string newDescripcion = mensaje;

            if (indexDescripcion > 0)
            {
                newDescripcion = mensaje.Substring(0, indexDescripcion);
                return newDescripcion;
            }

            return newDescripcion;
        }
    }
}

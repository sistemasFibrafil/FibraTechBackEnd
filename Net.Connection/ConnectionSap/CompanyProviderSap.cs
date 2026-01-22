using System;
using SAPbobsCOM;
using Net.Business.Entities;
using System.Runtime.InteropServices;
namespace Net.Connection
{
    public class CompanyProviderSap
    {
        private static Company _company;
        private readonly IConnectionSap _connection;
        private static readonly object _lock = new();
        private readonly ConnectionSapEntity _connectionConfig;

        public CompanyProviderSap(IConnectionSap connection, ConnectionSapEntity config)
        {
            _connection = connection;
            _connectionConfig = config;
        }

        public Company GetCompany()
        {
            lock (_lock)
            {
                if (_company == null || !_company.Connected)
                {
                    var result = _connection.ConnectToCompany(_connectionConfig);
                    if (result != "0")
                        throw new Exception($"Error al conectar SAP: {result}");

                    _company = RepositoryBaseSap.oCompany;
                }
                return _company;
            }
        }


        // <summary>
        /// Libera de forma segura una lista de objetos COM.
        /// </summary>
        /// <param name="comObjects">Array de objetos COM para liberar.</param>
        public void LiberarObjetosCOM(params object[] comObjects)
        {
            #pragma warning disable CA1416
            foreach (var obj in comObjects)
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                }
            }
            #pragma warning restore CA1416
        }
    }
}

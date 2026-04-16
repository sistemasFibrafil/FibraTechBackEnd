using System;
using SAPbobsCOM;
using Net.Business.Entities;
using System.Runtime.InteropServices;
namespace Net.Connection.ConnectionSAPBusinessOne
{
    public class CompanyProviderSAPBusinessOne
    {
        private static Company _company;
        private static readonly object _lock = new();
        private readonly IConnectionSAPBusinessOne _connection;
        private readonly ConnectionSapEntity _connectionConfig;

        public CompanyProviderSAPBusinessOne(IConnectionSAPBusinessOne connection, ConnectionSapEntity config)
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
                        throw new Exception($"Error al conectar SAP Business One: {result}");

                    _company = RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany;
                }
                return _company;
            }
        }

        public Company GetCompanyAuth()
        {
            lock (_lock)
            {
                if (_company != null && _company.Connected) _company.Disconnect();
                if (_company == null || !_company.Connected)
                {
                    ConnectionSapEntity connectionSapEntity2 = _connectionConfig;
                    connectionSapEntity2.UserName = "indirectcc";
                    connectionSapEntity2.Password = "1234";
                    var result = _connection.ConnectToCompany(connectionSapEntity2);
                    if (result != "0")
                        throw new Exception($"Error al conectar SAP Business One: {result}");

                    _company = RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany;
                }
                return _company;
            }
        }

        public void DisconnectCompany()
        {
            lock (_lock)
            {
                if (_company != null && _company.Connected)
                {
                    _company.Disconnect();
                    _company = null;
                }
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

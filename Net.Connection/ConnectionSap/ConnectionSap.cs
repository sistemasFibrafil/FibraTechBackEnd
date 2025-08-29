using System;
using SAPbobsCOM;
using Net.Business.Entities;
namespace Net.Connection
{
    public class ConnectionSap : IConnectionSap
    {
        public int ErrorCode;
        public string result;
        public string ErrorMensaje;
        public string ConnectToCompany(ConnectionSapEntity value)
        {
            try
            {
                if (RepositoryBaseSap.oCompany == null || !RepositoryBaseSap.oCompany.Connected)
                {
                    RepositoryBaseSap.oCompany = new Company
                    {
                        UseTrusted = false,
                        Server = value.Server,
                        LicenseServer = value.LicenseServer,
                        DbUserName = value.DbUserName,
                        DbPassword = value.DbPassword,
                        language = BoSuppLangs.ln_Spanish_La
                    };
                    RepositoryBaseSap.oCompany.CompanyDB = value.CompanyDB;
                    RepositoryBaseSap.oCompany.UserName = value.UserName;
                    RepositoryBaseSap.oCompany.Password = value.Password;

                    switch (value.DbServerType)
                    {
                        case "HANA":
                            RepositoryBaseSap.oCompany.DbServerType = BoDataServerTypes.dst_HANADB;
                            break;
                        case "SQL2008":
                            RepositoryBaseSap.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                            break;
                        case "SQL2012":
                            RepositoryBaseSap.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;
                            break;
                        case "SQL2014":
                            RepositoryBaseSap.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2014;
                            break;
                    }
                    //Se abre la conexion con SAP: AQUI SALE ERROR
                    result = RepositoryBaseSap.oCompany.Connect().ToString();

                    if (result != "0")
                    {
                        RepositoryBaseSap.oCompany.GetLastError(out ErrorCode, out ErrorMensaje);
                        result = ErrorMensaje;
                    }
                }
                else
                {
                    result = "0";
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public void DisConnectToCompany()
        {
            try
            {
                if (RepositoryBaseSap.oCompany.Connected)
                {
                    RepositoryBaseSap.oCompany.Disconnect();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

using System;
using SAPbobsCOM;
using Net.Business.Entities;
namespace Net.Connection.ConnectionSAPBusinessOne
{
    public class ConnectionSAPBusinessOne : IConnectionSAPBusinessOne
    {
        public int ErrorCode;
        public string result;
        public string ErrorMensaje;
        public string ConnectToCompany(ConnectionSapEntity value)
        {
            try
            {
                if (RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany == null || !RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.Connected)
                {
                    RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany = new Company
                    {
                        UseTrusted = false,
                        Server = value.Server,
                        LicenseServer = value.LicenseServer,
                        DbUserName = value.DbUserName,
                        DbPassword = value.DbPassword,
                        language = BoSuppLangs.ln_Spanish_La
                    };

                    RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.CompanyDB = value.CompanyDB;
                    RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.UserName = value.UserName;
                    RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.Password = value.Password;

                    switch (value.DbServerType)
                    {
                        case "SQL2008":
                            RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                            break;
                        case "SQL2012":
                            RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;
                            break;
                        case "SQL2014":
                            RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2014;
                            break;
                    }
                    //Se abre la conexion con SAP: AQUI SALE ERROR
                    result = RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.Connect().ToString();

                    if (result != "0")
                    {
                        RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.GetLastError(out ErrorCode, out ErrorMensaje);
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
                if (RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.Connected)
                {
                    RepositoryBaseSAPBusinessOne.RepositoryBaseSAPBusinessOne.oCompany.Disconnect();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

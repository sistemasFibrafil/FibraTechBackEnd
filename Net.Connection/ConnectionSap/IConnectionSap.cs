using Net.Business.Entities;
namespace Net.Connection
{
    public interface IConnectionSap
    {
        string ConnectToCompany(ConnectionSapEntity value);
        void DisConnectToCompany();
        void LiberarObjetosCOM(params object[] comObjects);
    }
}

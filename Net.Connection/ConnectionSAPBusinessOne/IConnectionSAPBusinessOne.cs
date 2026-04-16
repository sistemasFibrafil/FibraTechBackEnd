using Net.Business.Entities;
namespace Net.Connection.ConnectionSAPBusinessOne
{
    public interface IConnectionSAPBusinessOne
    {
        string ConnectToCompany(ConnectionSapEntity value);
        void DisConnectToCompany();
    }
}

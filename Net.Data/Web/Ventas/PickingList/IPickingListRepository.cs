using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
namespace Net.Data.Web
{
    public interface IPickingListRepository
    {
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListPickingPdfByDocEntry(int docEntry);
    }
}

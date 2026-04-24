using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
namespace Net.Data.Web
{
    public interface IPickingListRepository
    {
        Task<ResultadoTransaccionResponse<MemoryStream>> GetListPickingPdfByDocEntry(int docEntry);
    }
}

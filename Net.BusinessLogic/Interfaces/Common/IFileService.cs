using Microsoft.AspNetCore.Http;
namespace Net.BusinessLogic.Interfaces.Common
{
    public interface IFileService
    {
        string CreateTempFolder(string root);
        Task<string> SaveFileAsync(IFormFile file, string folder, string fileName);
        void MoveFile(string sourcePath, string destinationPath);
        void DeleteDirectory(string path);
    }
}

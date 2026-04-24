using Microsoft.AspNetCore.Http;
using Net.BusinessLogic.Interfaces.Common;
namespace Net.BusinessLogic.Services.Common
{
    public class FileService : IFileService
    {
        public string CreateTempFolder(string root)
        {
            var path = Path.Combine(root, Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder, string fileName)
        {
            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path;
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath, true);
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Common
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _userContentFolder;

        //private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
        }

        private async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);

            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string path)
        {
            var checkPath = Path.Combine(_userContentFolder, path);

            if (!Directory.Exists(checkPath))
            {
                Directory.CreateDirectory(checkPath);
            }

            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = Path.Combine(path, Guid.NewGuid() + originalFileName);
            await SaveFileAsync(file.OpenReadStream(), fileName);
            return Path.Combine("\\Uploads", fileName);
        }
    }
}

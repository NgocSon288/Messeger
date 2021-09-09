using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace App.Application.Common
{
    public interface IFileStorageService
    {
        Task DeleteFileAsync(string fileName);
        Task<string> SaveFileAsync(IFormFile file, string path);
    }
}
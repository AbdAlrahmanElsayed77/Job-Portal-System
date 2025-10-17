using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using BL.Contracts;
using Microsoft.AspNetCore.Http;

namespace BL.Services
{
    public class FileService : IFileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadFileAsync(string folderName, IFormFile file)
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", folderName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the full URL instead of just the name
                return GetFileUrl(folderName, fileName);
            }
            catch (Exception ex)
            {
                return $"Error uploading file: {ex.Message}";
            }
        }

        public async Task<string> RemoveFileAsync(string folderName, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", folderName, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return "File deleted successfully.";
                }

                return "File not found.";
            }
            catch (Exception ex)
            {
                return $"Error deleting file: {ex.Message}";
            }
        }

        public string GetFileUrl(string folderName, string fileName)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) return fileName; // fallback if no context available

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/Files/{folderName}/{fileName}";
        }

    }
}

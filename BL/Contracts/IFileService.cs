using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(string folderName, IFormFile file);
        Task<string> RemoveFileAsync(string folderName, string fileName);
        string GetFileUrl(string folderName, string fileName);

    }
}

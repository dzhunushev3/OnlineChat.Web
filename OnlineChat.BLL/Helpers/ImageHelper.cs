using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.BLL.Helpers
{
    public static class ImageHelper
    {
        public static async Task<string> SaveImageAndGetFullPath(string userId, IFormFile formFile, int flag,
            IHostingEnvironment hostingEnvironment)
        {
            string directoryPath = Path.Combine(hostingEnvironment.WebRootPath, "images", flag == 1 ? "avatars" : "publications");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            Guid? guid = Guid.NewGuid();
            var isGuid = (flag == 0 ? guid.ToString() : "");
            var filePath = Path.Combine(directoryPath, userId + isGuid + GetImageExtension(formFile.FileName));
            var relativeFilePath = Path.Combine("images", flag == 1 ? "avatars" : "publications", userId + isGuid + GetImageExtension(formFile.FileName));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return relativeFilePath;
        }

        public static string GetImageExtension(string fileName)
        {
            var parts = fileName.Split(".", StringSplitOptions.RemoveEmptyEntries);
            return "." + parts[parts.Length - 1];
        }
    }
}

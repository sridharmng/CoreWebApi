using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CoreWebApi.Helpers
{
    public static class FileHelper
    {
        public static bool CheckIfImageFile(IFormFile file)
        {

            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".jpeg" || extension == ".jpg" || extension == ".JPG" || extension == ".png");

        }
        public static async Task<string> WriteFile(IFormFile picture)
        {
            bool isSaveSuccess = false;
            string fileName = "";
            string dbPath = "";
            string fullPath = "";
            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);
                if (picture.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(picture.ContentDisposition).FileName.Trim('"');
                    fullPath = Path.Combine(pathToSave, fileName);
                    dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await picture.CopyToAsync(stream);
                    }
                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
            }
            return dbPath;
        }

        public static void DeleteFile(string imageToBeDelete)
        {
            if ((System.IO.File.Exists(imageToBeDelete)))
            {
                System.IO.File.Delete(imageToBeDelete);
            }
        }

    }
}

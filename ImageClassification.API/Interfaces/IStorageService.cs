using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.API.Interfaces
{
    public interface IStorageService
    {
        void DeleteStoredImage(string folder, string classification, int index);
        IEnumerable<StorageFolderClassificationVM> GetStorageFolderClassifications(string folder);
        IEnumerable<StorageFolderVM> GetStorageFolders();
        string GetStoredImage(string folder, string classification, int index);
        IEnumerable<StoredImageVM> GetStoredImages(string folder, string classification);
        Task<string> UploadImage(IFormFile imageFile, string folder, string classification);
    }
}
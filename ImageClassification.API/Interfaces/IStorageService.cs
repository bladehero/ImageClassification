using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageClassification.API.Interfaces
{
    public interface IStorageService
    {
        void DeleteStoredImage(string folder, string classification, int index);
        void DeleteFolder(string folder, string classification = null, bool deleteContent = true);
        void CreateFolder(string folder);
        void MoveFolder(string folder, string destination);
        IEnumerable<StorageFolderClassificationVM> GetStorageFolderClassifications(string folder);
        IEnumerable<StorageFolderVM> GetStorageFolders();
        string GetStoredImage(string folder, string classification, int index);
        IEnumerable<StoredImageVM> GetStoredImages(string folder, string classification);
        IAsyncEnumerable<string> UploadImages(IEnumerable<IFormFile> imageFile, string folder, string classification);
    }
}
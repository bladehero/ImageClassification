using ImageClassification.API.Configurations;
using ImageClassification.API.Exceptions;
using ImageClassification.API.Extensions;
using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageClassification.API.Services
{
    public class StorageService : IStorageService
    {
        private readonly ILogger<ImageSourceService> _logger;
        private readonly StorageOptions _storageOptions;
        private readonly ImageSourceUploadOptions _sourceUploadOptions;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly Regex _regex;
        public StorageService(ILogger<ImageSourceService> logger,
                              IOptions<StorageOptions> storageOptions,
                              IOptions<ImageSourceUploadOptions> sourceUploadOptions,
                              IHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _storageOptions = storageOptions.Value;
            _sourceUploadOptions = sourceUploadOptions.Value;
            _hostingEnvironment = hostingEnvironment;
            _regex = new Regex($"{_sourceUploadOptions.StringBefore}\\{_sourceUploadOptions.LeftBracer}(.*?)\\{_sourceUploadOptions.RightBracer}{_sourceUploadOptions.StringAfter}");
        }

        public IEnumerable<StorageFolderVM> GetStorageFolders()
        {
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Storage directory not found!");
            }

            var folders = Directory.GetDirectories(path);
            return folders.Select(x => new StorageFolderVM
            {
                Name = x.Replace(path, string.Empty),
                FileCount = Directory.GetFiles(x, "*.*", SearchOption.AllDirectories).Length
            });
        }

        public void CreateFolder(string folder)
        {
            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Storage directory not found!");
            }

            var folderPath = Path.Join(path, folder);
            if (Directory.Exists(folderPath))
            {
                throw new IOException($"Directory `{folder}` already exists");
            }

            Directory.CreateDirectory(folderPath);
        }

        public void MoveFolder(string folder, string destination)
        {
            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Storage directory not found!");
            }

            var folderPath = Path.Join(path, folder);
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Directory `{folder}` not found");
            }

            var destinationPath = Path.Join(path, destination);
            if (Directory.Exists(destinationPath))
            {
                throw new IOException($"Directory `{folder}` cannot be moved to `{destination}`, because it already exists");
            }

            Directory.Move(folderPath, destinationPath);
        }

        public void DeleteFolder(string folder, bool deleteContent = true)
        {
            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Storage directory not found!");
            }

            var folderPath = Path.Join(path, folder);
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Directory `{folder}` not found");
            }

            if (!deleteContent && Directory.EnumerateFileSystemEntries(folderPath).Any())
            {
                throw new IOException($"Directory `{folder}` contains other entries");
            }

            Directory.Delete(folderPath, true);
        }

        public IEnumerable<StorageFolderClassificationVM> GetStorageFolderClassifications(string folder)
        {
            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath);
            var folderPath = Path.Join(path, folder);
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Folder `{folder}` not found!");
            }

            var folders = Directory.GetDirectories(folderPath);
            return folders.Select(x => new StorageFolderClassificationVM
            {
                Folder = folder,
                Classification = Path.GetFileName(x),
                FileCount = Directory.GetFiles(x, "*.*", SearchOption.AllDirectories).Length
            });
        }
        public IEnumerable<StoredImageVM> GetStoredImages(string folder, string classification)
        {
            folder = ValidateSingleFolder(folder);
            classification = ValidateSingleFolder(classification);

            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath, folder, classification);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Classification `{classification}` not found!");
            }

            var files = Directory.GetFiles(path);

            return files.Where(x => Path.GetFileNameWithoutExtension(x).StartsWith(classification)).Select(x =>
             {
                 return new StoredImageVM
                 {
                     Classification = classification,
                     Folder = folder,
                     Index = GetFileIndex(x),
                     Name = Path.GetFileName(x)
                 };
             });
        }
        public string GetStoredImage(string folder, string classification, int index)
        {
            var filePath = GetImageAndValidateHelper(folder, classification, index);

            return filePath;
        }
        public void DeleteStoredImage(string folder, string classification, int index)
        {
            var filePath = GetImageAndValidateHelper(folder, classification, index);

            File.Delete(filePath);

            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath, folder, classification);
            var files = Directory.GetFiles(path)
                                 .Select(x => new { Path = x, Index = GetFileIndex(x) })
                                 .OrderBy(x => x.Index)
                                 .Skip(index);

            foreach (var file in files)
            {
                var currentIndex = index++;
                var pattern = currentIndex == 0 ? string.Empty : _sourceUploadOptions.Build(currentIndex);
                var updated = _regex.Replace(file.Path, pattern);
                File.Move(file.Path, updated);
            }
        }
        public async Task<string> UploadImage(IFormFile imageFile, string folder, string classification)
        {
            if (imageFile.Length == 0)
                throw new EmptyFileException();

            folder = ValidateSingleFolder(folder);
            classification = ValidateSingleFolder(classification);

            var imageMemoryStream = new MemoryStream();
            await imageFile.CopyToAsync(imageMemoryStream);

            var imageData = imageMemoryStream.ToArray();
            if (!imageData.IsValidImage())
            {
                throw new ImageFormatException(ImageExtensions.CanBeUsed.Select(x => x.GetDescription()));
            }

            _logger.LogInformation("Started uploading process...");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var extension = Path.GetExtension(imageFile.FileName);
            var name = Path.ChangeExtension(classification, extension);
            var path = Path.Combine(_storageOptions.StoragePath, folder, classification);
            Directory.CreateDirectory(path);
            var fileName = Path.Join(path, name);
            var index = 1;

            using (var stream = imageFile.OpenReadStream())
            {
                var files = Directory.GetFiles(path);
                while (files.Any(x => Path.GetFileNameWithoutExtension(NormalizePath(x)) == Path.GetFileNameWithoutExtension(NormalizePath(fileName))))
                {
                    fileName = Path.Combine(path, Path.ChangeExtension($"{classification}{_sourceUploadOptions.Build(index++)}", extension));
                }

                fileName = fileName.Replace("\\", "/");
                using var fs = new FileStream(fileName, FileMode.CreateNew);
                await stream.CopyToAsync(fs);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation($"Image uploaded in {elapsedMs} miliseconds");

            var result = fileName.TrimStart(_storageOptions.StoragePath);
            return result;
        }

        #region Helpers
        private string GetImageAndValidateHelper(string folder, string classification, int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index of image classification can be 0 or greater!");
            }

            folder = ValidateSingleFolder(folder);
            classification = ValidateSingleFolder(classification);

            var path = Path.Join(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath, folder, classification);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Classification `{classification}` not found!");
            }

            var file = index == 0 ? $"{classification}" : $"{classification}{_sourceUploadOptions.Build(index)}";
            var filePath = Directory.GetFiles(path).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == file);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new FileNotFoundException("File by parameters wasn't found", file);
            }

            return filePath;
        }
        private static string ValidateSingleFolder(string folder)
        {
            folder = Path.Join(folder.Trim(), null);
            if (string.IsNullOrWhiteSpace(folder)
                || !Path.GetDirectoryName(folder).Equals(string.Empty)
                || !Path.GetExtension(folder).Equals(string.Empty))
            {
                throw new ArgumentException($"Folder shouldn't be empty and must be a single section! As example: `My Folder`.", nameof(folder));
            }

            return folder;
        }
        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        private int GetFileIndex(string path)
        {
            var indexString = _regex.Matches(path).Select(x => x.Groups).SelectMany(x => x.Cast<Group>()).LastOrDefault()?.Value;
            _ = int.TryParse(indexString, out int index);
            return index;
        }
        #endregion
    }
}

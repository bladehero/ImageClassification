using ImageClassification.API.Configurations;
using ImageClassification.API.Delegates;
using ImageClassification.API.Enums;
using ImageClassification.API.Exceptions;
using ImageClassification.API.Extensions;
using ImageClassification.API.Interfaces;
using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassification.API.Services
{
    public class ImageSourceService : IImageSourceService
    {
        private readonly ILogger<ImageSourceService> _logger;
        private readonly ImageParsingResolver _imageParsingResolver;
        private readonly IParsingContext _parsingContext;
        private readonly StorageOptions _storageOptions;
        private readonly ImageSourceUploadOptions _sourceUploadOptions;

        public ImageSourceService(ILogger<ImageSourceService> logger,
                                  ImageParsingResolver imageParsingResolver,
                                  IParsingContext parsingContext,
                                  IOptions<StorageOptions> storageOptions,
                                  IOptions<ImageSourceUploadOptions> sourceUploadOptions)
        {
            _logger = logger;
            _imageParsingResolver = imageParsingResolver;
            _parsingContext = parsingContext;
            _storageOptions = storageOptions.Value;
            _sourceUploadOptions = sourceUploadOptions.Value;
        }

        public void ChangeParsingStrategy(ImageParsingStrategy key)
        {
            if (_imageParsingResolver?.Invoke(key) is IImageParsingStrategy strategy)
            {
                _parsingContext.ImageParsingStrategy = strategy;
            }
        }

        public async Task<ImageResult> ParseSingleImageAsync(string keyword, int index)
        {
            var result = await _parsingContext.ParseImageAsync(keyword, index);
            return result;
        }

        public async Task<string> UploadSingleImageAsync(IFormFile imageFile, string folder, string classification)
        {
            if (imageFile.Length == 0)
                throw new EmptyFileException();

            folder = Path.Join(folder.Trim(), null);
            if (string.IsNullOrWhiteSpace(folder)
                || !Path.GetDirectoryName(folder).Equals(string.Empty)
                || !Path.GetExtension(folder).Equals(string.Empty))
            {
                throw new ArgumentException($"Folder shouldn't be empty and must be a single section! As example: `My Folder`.", nameof(folder));
            }

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
                while (File.Exists(fileName))
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
    }
}

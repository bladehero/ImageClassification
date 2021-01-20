using ImageClassification.API.Configurations;
using ImageClassification.API.Delegates;
using ImageClassification.API.Enums;
using ImageClassification.API.Extensions;
using ImageClassification.Core.Preparation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers.Images
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController : Controller
    {
        private readonly ILogger<SourceController> _logger;
        private readonly ImageParsingResolver _imageParsingResolver;
        private readonly IParsingContext _parsingContext;
        private readonly StorageOptions _storageOptions;
        private readonly ImageSourceUploadOptions _sourceUploadOptions;

        public SourceController(ILogger<SourceController> logger,
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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values.TryGetValue("source", out object value))
                if (Enum.TryParse(value?.ToString(), out ImageParsingStrategy key))
                    if (_imageParsingResolver?.Invoke(key) is IImageParsingStrategy strategy)
                        _parsingContext.ImageParsingStrategy = strategy;

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Gets an image by keyword and index.
        /// </summary>
        /// <param name="source">Type of source to use.</param>
        /// <param name="keyword">Keyword for search.</param>
        /// <param name="index">Index as a position of searching</param>
        /// <returns>Stream result as <see cref="FileStreamResult">FileStream</see>.</returns>
        [HttpGet("{source:ImageParsingStrategy}")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(ImageParsingStrategy source, string keyword, int index)
        {
            try
            {
                var result = await _parsingContext.ParseImageAsync(keyword, index);
                return File(result.Stream, result.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogErrorWithName(ex);
                throw;
            }
        }


        /// <summary>
        /// Uploads image to folder for future trainnings.
        /// </summary>
        /// <param name="imageFile">Image file.</param>
        /// <param name="folder">Folder where file should be uploaded. Folder should be a single section.</param>
        /// <param name="classification">Classification label.</param>
        /// <returns>Returns file name if everything went well.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(415)]
        public async Task<IActionResult> Post([Required] IFormFile imageFile, [Required] string folder, [Required] string classification)
        {
            if (imageFile.Length == 0)
                return BadRequest();

            folder = Path.Join(folder.Trim(), null);
            if (string.IsNullOrWhiteSpace(folder)
                || !Path.GetDirectoryName(folder).Equals(string.Empty)
                || !Path.GetExtension(folder).Equals(string.Empty))
            {
                return BadRequest($"Folder shouldn't be empty and must be a single section! As example: `My Folder`.");
            }

            var imageMemoryStream = new MemoryStream();
            await imageFile.CopyToAsync(imageMemoryStream);

            // Check that the image is valid.
            var imageData = imageMemoryStream.ToArray();
            if (!imageData.IsValidImage())
                return StatusCode(StatusCodes.Status415UnsupportedMediaType);

            _logger.LogInformation("Started uploading process...");

            // Measure execution time.
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var extension = Path.GetExtension(imageFile.FileName);
            var name = Path.ChangeExtension(classification, extension);
            var path = Path.Combine(_storageOptions.StoragePath, folder, classification);
            Directory.CreateDirectory(path);
            var fileName = Path.Join(path, name);
            var index = 1;

            using (var stream = imageFile.OpenReadStream())
            {
                while (System.IO.File.Exists(fileName))
                {
                    fileName = Path.Combine(path, Path.ChangeExtension($"{classification}{_sourceUploadOptions.Build(index++)}", extension));
                }

                fileName = fileName.Replace("\\", "/");
                using var fs = new FileStream(fileName, FileMode.CreateNew);
                await stream.CopyToAsync(fs);
            }

            // Stop measuring time.
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation($"Image uploaded in {elapsedMs} miliseconds");

            var result = fileName.TrimStart(_storageOptions.StoragePath);
            return Ok(result);
        }
    }
}

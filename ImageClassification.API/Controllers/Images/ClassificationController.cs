using ImageClassification.API.Configurations;
using ImageClassification.API.Exceptions;
using ImageClassification.API.Extensions;
using ImageClassification.API.Global;
using ImageClassification.API.Models;
using ImageClassification.Core.Train;
using ImageClassification.Shared.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers.Images
{
    [Route("api/[controller]/{classifier:ClassifierName}")]
    [ApiController]
    public class ClassificationController : Controller
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IConfiguration _configuration;
#pragma warning restore IDE0052 // Remove unread private members
        private readonly ILogger<ClassificationController> _logger;
        private readonly MLModelOptions _mlOptions;
        private PredictionEngine<InMemoryImageData, ImagePrediction> _predictionEngine;

        public ClassificationController(IConfiguration configuration,
                                        ILogger<ClassificationController> logger,
                                        IOptions<MLModelOptions> mlOptions)
        {
            //_predictionEnginePool = predictionEnginePool;
            _configuration = configuration;
            _logger = logger;
            _mlOptions = mlOptions.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values.TryGetValue("classifier", out object value))
                if (value is string classifier)
                {
                    var path = Path.Combine(_mlOptions.MLModelFilePath, Path.ChangeExtension(classifier, Constants.Extensions.Zip));
                    var mlContext = new MLContext();
                    var trainedModel = mlContext.Model.Load(path, out DataViewSchema schema);
                    _predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(trainedModel);
                }

            if (_predictionEngine is null)
            {
                throw new NotFoundClassifierException($"Classifier `{value ?? "null"}` is not found!");
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// If exists classifier, returns all classifications that classifier can produce.
        /// </summary>
        /// <param name="classifier">Name of classifier.</param>
        /// <returns>Collection of possible classifications.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IAsyncEnumerable<string> Get(string classifier)
        {
            if (string.IsNullOrWhiteSpace(classifier))
            {
                throw new ArgumentException($"'{nameof(classifier)}' cannot be null or whitespace", nameof(classifier));
            }

            var path = _mlOptions.MLModelFilePath;
            var fileName = Path.Combine(path, Path.ChangeExtension(classifier, Constants.Extensions.Zip));
            if (!System.IO.File.Exists(fileName))
            {
                throw new FileNotFoundException($"Classifier with the name `{classifier}` not found!", fileName);
            }

            var zip = ZipFile.OpenRead(fileName);
            var classifications = zip.GetEntry(DefaultTrainWrapper.ClassificationsFileName)
                                     .Open()
                                     .ReadAllLinesAsync();
            return classifications;
        }

        /// <summary>
        /// Defines the category of provided image.
        /// </summary>
        /// <param name="classifier">Name of classifier.</param>
        /// <param name="imageFile">Image file.</param>
        /// <returns>String as name classification.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ClassificationPrediction), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post(string classifier, IFormFile imageFile)
        {
            if (imageFile.Length == 0)
                return BadRequest();

            var imageMemoryStream = new MemoryStream();
            await imageFile.CopyToAsync(imageMemoryStream);

            // Check that the image is valid.
            var imageData = imageMemoryStream.ToArray();
            if (!imageData.IsValidImage())
                return StatusCode(StatusCodes.Status415UnsupportedMediaType);

            _logger.LogInformation("Start processing image...");

            // Measure execution time.
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Set the specific image data into the ImageInputData type used in the DataView.
            var imageInputData = new InMemoryImageData(image: imageData, label: null, imageFileName: null);

            // Predict code for provided image.
            var prediction = _predictionEngine.Predict(imageInputData);

            // Stop measuring time.
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation($"Image processed in {elapsedMs} miliseconds");

            // Predict the image's label (The one with highest probability).
            var imageBestLabelPrediction = new ClassificationPrediction
            {
                PredictedLabel = prediction.PredictedLabel,
                Probability = prediction.Score.Max(),
                PredictionExecutionTime = elapsedMs,
                ImageId = imageFile.FileName,
            };

            return Ok(imageBestLabelPrediction);
        }
    }
}

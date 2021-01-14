using ImageClassification.API.Extensions;
using ImageClassification.API.Models;
using ImageClassification.Shared.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageClassificationController : ControllerBase
    {
        private readonly PredictionEnginePool<InMemoryImageData, ImagePrediction> _predictionEnginePool;
        private readonly ILogger<ImageClassificationController> _logger;

        public ImageClassificationController(PredictionEnginePool<InMemoryImageData, ImagePrediction> predictionEnginePool, IConfiguration configuration, ILogger<ImageClassificationController> logger) //When using DI/IoC
        {
            _predictionEnginePool = predictionEnginePool;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("classify")]
        public async Task<IActionResult> Classify(IFormFile imageFile)
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
            var prediction = _predictionEnginePool.Predict(imageInputData);

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

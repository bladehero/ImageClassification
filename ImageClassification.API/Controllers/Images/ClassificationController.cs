using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using ImageClassification.Shared.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers.Images
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationController : Controller
    {
        private readonly IPredictionEnginePoolService<InMemoryImageData, ImagePrediction> _predictionEnginePoolService;
        private readonly IClassificationService _classificationService;

        public ClassificationController(IPredictionEnginePoolService<InMemoryImageData, ImagePrediction> predictionEnginePoolService,
                                        IClassificationService classificationService)
        {
            _predictionEnginePoolService = predictionEnginePoolService;
            _classificationService = classificationService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values.TryGetValue("classifier", out object value))
            {
                if (value is string classifier)
                {
                    if (!_predictionEnginePoolService.ContainsKey(classifier))
                    {
                        _predictionEnginePoolService.Add(classifier);
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// If exists classifier, returns all classifications that classifier can produce.
        /// </summary>
        /// <param name="classifier">Name of classifier.</param>
        /// <returns>Collection of possible classifications.</returns>
        [HttpGet("{classifier:ClassifierName}")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Get(string classifier)
        {
            var classifications = _classificationService.GetPossibleClassifications(classifier);
            return Ok(classifications);
        }

        /// <summary>
        /// Defines the category of provided image.
        /// </summary>
        /// <param name="classifier">Name of classifier.</param>
        /// <param name="image">Image file.</param>
        /// <returns>String as name classification.</returns>
        [HttpPost("{classifier:ClassifierName}")]
        [ProducesResponseType(typeof(ClassificationPredictionVM), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post(string classifier, IFormFile image)
        {
            var classification = await _classificationService.Classify(classifier, image);
            return Ok(classification);
        }
    }
}

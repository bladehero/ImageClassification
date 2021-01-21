using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassifiersController : ControllerBase
    {
        private readonly IClassifierService _classifierService;

        public ClassifiersController(IClassifierService classifierService)
        {
            _classifierService = classifierService;
        }

        /// <summary>
        /// Enumerate all classifiers are stored.
        /// </summary>
        /// <returns>Collection of classifiers' names.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IEnumerable<string> Get()
        {
            return _classifierService.GetAllClassifiers();
        }

        /// <summary>
        /// Downloads a .zip file which represents TensorFlow model of classifier.
        /// </summary>
        /// <param name="classifier">Classifier name.</param>
        /// <returns>Byte array (file) for specified classifier, if exists.</returns>
        [HttpGet("download/{classifier}")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public FileResult Download([Required] string classifier)
        {
            var file = _classifierService.GetClassifierStream(classifier);
            return File(file, System.Net.Mime.MediaTypeNames.Application.Zip);
        }

        /// <summary>
        /// Downloads a .zip file which represents TensorFlow model of classifier.
        /// </summary>
        /// <param name="imageFolder">Image folder.</param>
        /// <param name="classifier">Classifier name.</param>
        /// <returns>Byte array (file) for specified classifier, if exists.</returns>
        [HttpPatch("train/{classifier}")]
        [ProducesResponseType(typeof(TimeSpan?), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Train([Required] string imageFolder, [Required] string classifier)
        {
            var trainWrapper = await _classifierService.TrainClassifier(imageFolder, classifier);

            return Ok(trainWrapper.Stopwatch?.Elapsed);
        }
    }
}

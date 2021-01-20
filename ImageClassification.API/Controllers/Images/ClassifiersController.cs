using ImageClassification.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ImageClassification.API.Controllers.Images
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassifiersController : ControllerBase
    {
        private readonly IClassificationService _classificationService;

        public ClassifiersController(IClassificationService classificationService)
        {
            _classificationService = classificationService;
        }

        /// <summary>
        /// Enumerate all classifiers are stored.
        /// </summary>
        /// <returns>Collection of classifiers' names.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IEnumerable<string> Get()
        {
            return _classificationService.GetAllClassifiers();
        }
    }
}

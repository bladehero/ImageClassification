using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IEnumerable<string> Get()
        {
            return _classificationService.GetAllClassifiers();
        }
    }
}

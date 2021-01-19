using ImageClassification.API.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageClassification.API.Controllers.Images
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassifiersController : ControllerBase
    {
        private readonly ILogger<ClassifiersController> _logger;
        private readonly MLModelOptions _mlOptions;

        public ClassifiersController(ILogger<ClassifiersController> logger,
                                     IOptions<MLModelOptions> mlOptions)
        {
            _logger = logger;
            _mlOptions = mlOptions.Value;
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
            var path = _mlOptions.MLModelFilePath;
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Not found repository for storaging classifiers");
            }

            var classifiers = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);
            if (classifiers.Length == 0)
            {
                throw new FileNotFoundException("System has no classifiers yet!");
            }

            return classifiers.Select(path => Path.GetFileNameWithoutExtension(path));
        }
    }
}

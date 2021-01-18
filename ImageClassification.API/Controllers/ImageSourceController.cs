using ImageClassification.API.Delegates;
using ImageClassification.API.Enums;
using ImageClassification.API.Extensions;
using ImageClassification.Core.Preparation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers
{
    [Route("api/[controller]/{source:ImageParsingStrategy}")]
    [ApiController]
    public class ImageSourceController : Controller
    {
        private readonly ILogger<ImageClassificationController> _logger;
        private readonly ImageParsingResolver _imageParsingResolver;
        private readonly IParsingContext _parsingContext;

        public ImageSourceController(ILogger<ImageClassificationController> logger,
                                     ImageParsingResolver imageParsingResolver,
                                     IParsingContext parsingContext)
        {
            _logger = logger;
            _imageParsingResolver = imageParsingResolver;
            _parsingContext = parsingContext;
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
        /// <returns>Stream result as <see cref="FileStreamResult">FileStreamResult</see>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
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
    }
}

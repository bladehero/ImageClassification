using ImageClassification.API.Enums;
using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController : ControllerBase
    {
        private readonly IImageSourceService _imageSourceService;

        public SourceController(IImageSourceService imageSourceService)
        {
            _imageSourceService = imageSourceService;
        }

        /// <summary>
        /// Gets an image by keyword and index.
        /// </summary>
        /// <param name="source">Image parsing strategy.</param>
        /// <param name="keyword">Keyword for search.</param>
        /// <param name="index">Index as a position of searching</param>
        /// <remarks>
        /// Can be used with a parameter `source` for using specific strategies.
        /// </remarks>
        /// <returns>Stream result as <see cref="FileStreamResult">FileStream</see>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string keyword, int index, ImageParsingStrategy source = ImageParsingStrategy.DefaultImageParsing)
        {
            _imageSourceService.ChangeParsingStrategy(source);
            var imageResult = await _imageSourceService.ParseSingleImageAsync(keyword, index);
            return File(imageResult.Stream, imageResult.ContentType);
        }
    }
}

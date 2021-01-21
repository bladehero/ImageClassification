using ImageClassification.API.Enums;
using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ImageClassification.API.Controllers.Images
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
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string keyword, int index, ImageParsingStrategy source = ImageParsingStrategy.DefaultImageParsing)
        {
            _imageSourceService.ChangeParsingStrategy(source);
            var imageResult = await _imageSourceService.ParseSingleImageAsync(keyword, index);
            return File(imageResult.Stream, imageResult.ContentType);
        }

        /// <summary>
        /// Uploads image to folder for future trainnings.
        /// </summary>
        /// <param name="image">Image file.</param>
        /// <param name="folder">Folder where file should be uploaded. Folder should be a single section.</param>
        /// <param name="classification">Classification label.</param>
        /// <returns>Returns file name if everything went well.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([Required] IFormFile image, [Required] string folder, [Required] string classification)
        {
            var fileName = await _imageSourceService.UploadSingleImageAsync(image, folder, classification);
            return Ok(fileName);
        }
    }
}

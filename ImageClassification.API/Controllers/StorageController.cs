using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

#nullable enable

namespace ImageClassification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// Shows all available folders in storage.
        /// </summary>
        /// <returns>Collection of <see cref="StorageFolderVM">StorageFolderVM</see> with additional info.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StorageFolderVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            var folders = _storageService.GetStorageFolders();
            return Ok(folders);
        }

        /// <summary>
        /// Shows all classification folders and their info for specisfied folder.
        /// </summary>
        /// <param name="folder">Folder to search to.</param>
        /// <returns>Collection of <see cref="StorageFolderClassificationVM">StorageFolderClassificationVM</see>.</returns>
        [HttpGet("{folder}")]
        [ProducesResponseType(typeof(IEnumerable<StorageFolderClassificationVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Get([Required] string folder)
        {
            var classifications = _storageService.GetStorageFolderClassifications(folder);
            return Ok(classifications);
        }

        /// <summary>
        /// Shows information related to the image with indexing.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="classification"></param>
        /// <returns></returns>
        [HttpGet("{folder}/{classification}")]
        [ProducesResponseType(typeof(IEnumerable<StoredImageVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Get([Required] string folder, string classification)
        {
            var images = _storageService.GetStoredImages(folder, classification);
            return Ok(images);
        }

        [HttpGet("download/{folder}/{classification}/{index}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Get([Required] string folder, [Required] string classification, [Required] int index)
        {
            var file = _storageService.GetStoredImage(folder, classification, index);
            new FileExtensionContentTypeProvider().TryGetContentType(file, out string contentType);
            return PhysicalFile(file, contentType);
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
            var fileName = await _storageService.UploadImage(image, folder, classification);
            return Ok(fileName);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete([Required] string folder, [Required] string classification, [Required] int index)
        {
            _storageService.DeleteStoredImage(folder, classification, index);
            return Ok();
        }
    }
}

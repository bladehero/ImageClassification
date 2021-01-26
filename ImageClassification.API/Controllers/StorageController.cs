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
        /// <param name="folder">Folder to search to.</param>
        /// <param name="classification">Image classification.</param>
        /// <returns>Information of stored image.</returns>
        [HttpGet("{folder}/{classification}")]
        [ProducesResponseType(typeof(IEnumerable<StoredImageVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Get([Required] string folder, string classification)
        {
            var images = _storageService.GetStoredImages(folder, classification);
            return Ok(images);
        }

        /// <summary>
        /// Downloads specified by parameters image.
        /// </summary>
        /// <param name="folder">Folder of storing.</param>
        /// <param name="classification">Image classification.</param>
        /// <param name="index">Index of image.</param>
        /// <returns>Stream for specified image, if it exists.</returns>
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
        /// Creates folder with a name.
        /// </summary>
        /// <param name="folder">Folder name.</param>
        /// <returns>Success if folder is created.</returns>
        [HttpPost("{folder}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Post([Required] string folder)
        {
            _storageService.CreateFolder(folder);
            return Ok();
        }

        /// <summary>
        /// Uploads image to folder for future trainnings.
        /// </summary>
        /// <param name="image">Image file.</param>
        /// <param name="folder">Folder where file should be uploaded. Folder should be a single section.</param>
        /// <param name="classification">Classification label.</param>
        /// <returns>Returns file name if everything went well.</returns>
        [HttpPost("upload/{folder}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([Required] string folder, [Required] IFormFile image, [Required] string classification)
        {
            var fileName = await _storageService.UploadImage(image, folder, classification);
            return Ok(fileName);
        }

        /// <summary>
        /// Renames folder with a new name.
        /// </summary>
        /// <param name="folder">Current folder name.</param>
        /// <param name="newName">New folder name.</param>
        /// <returns>Success if folder is renamed.</returns>
        [HttpPut("{folder}/{newName}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Put([Required] string folder, [Required] string newName)
        {
            _storageService.MoveFolder(folder, newName);
            return Ok();
        }

        /// <summary>
        /// Removes folder from a list.
        /// </summary>
        /// <param name="folder">Folder where file should be uploaded. Folder should be a single section.</param>
        /// <param name="deleteContent">If true - recursively removes content from folder, otherwise only removes folder if empty.</param>
        /// <returns>Success if image is removed.</returns>
        [HttpDelete("{folder}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorVM), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete([Required] string folder, bool deleteContent = true)
        {
            _storageService.DeleteFolder(folder, deleteContent);
            return Ok();
        }

        /// <summary>
        /// Removes image from classification.
        /// </summary>
        /// <param name="folder">Folder where file should be uploaded. Folder should be a single section.</param>
        /// <param name="classification">Image classification.</param>
        /// <param name="index">Index of image.</param>
        /// <returns>Success if image is removed.</returns>
        [HttpDelete("images/{folder}/{classification}/{index:int}")]
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

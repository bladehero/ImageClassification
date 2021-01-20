using ImageClassification.API.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ImageClassification.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;
        private readonly IExceptionMapper _exceptionMapper;

        public const string DefaultMessage = "Internal server error";

        public ErrorsController(ILogger<ErrorsController> logger, IExceptionMapper exceptionMapper)
        {
            _logger = logger;
            _exceptionMapper = exceptionMapper;
        }

        [Route("error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            if (exception is null)
            {
                return NotFound();
            }

            var error = _exceptionMapper.Map(exception);

            _logger.LogError(exception, error?.Message);

            return StatusCode(error?.StatusCode ?? StatusCodes.Status500InternalServerError,
                              error?.Data ?? error?.Message ?? DefaultMessage);
        }
    }
}

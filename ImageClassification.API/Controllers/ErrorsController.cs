using ExceptionMapper.Interfaces;
using ImageClassification.API.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#nullable enable

namespace ImageClassification.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;
        private readonly IExceptionMapper _exceptionMapper;

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

            IErrorData error = _exceptionMapper.Map(exception);

            _logger.LogError(exception, error.Message);

            var model = new ErrorVM
            {
                Message = error.Message,
                Data = error.Data
            };
            return StatusCode(error.StatusCode, model);
        }
    }
}

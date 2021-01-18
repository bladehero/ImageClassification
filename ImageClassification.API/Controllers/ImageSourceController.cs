﻿using ImageClassification.API.Delegates;
using ImageClassification.API.Enums;
using ImageClassification.API.Extensions;
using ImageClassification.Core.Preparation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
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
                if (Enum.TryParse(value?.ToString(), out ImageParsingStartegy key))
                    if (_imageParsingResolver?.Invoke(key) is IImageParsingStrategy strategy)
                        _parsingContext.ImageParsingStrategy = strategy;

            base.OnActionExecuting(context);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string keyword, int index)
        {
            try
            {
                using var image = await _parsingContext.ParseImageAsync(keyword, index);
                var stream = image.ToStream();
                var contentType = image.RawFormat.ToContentType();
                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogErrorWithName(ex);
                throw;
            }
        }
    }
}

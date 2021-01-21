using ExceptionMapper.Extensions;
using ExceptionMapper.Interfaces;
using ImageClassification.API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ImageClassification.API.Extensions
{
    public static class ExceptionMapperExtensions
    {
        public static IServiceCollection AddCustomExceptionMapper(this IServiceCollection services)
        {
            services.ConfigureExceptionMapper(builder =>
            {
                #region 400
                builder.Configure((EmptyFileException ex) =>
                    new ErrorData(StatusCodes.Status400BadRequest, ex.Message));

                builder.Configure((ArgumentException ex) =>
                    new ErrorData(StatusCodes.Status400BadRequest, ex.Message, new { Argument = ex.ParamName }));

                builder.Configure((ArgumentNullException ex) =>
                    new ErrorData(StatusCodes.Status400BadRequest, ex.Message, new { Argument = ex.ParamName }));
                #endregion

                #region 404
                builder.Configure((NotFoundClassifierException ex) =>
                    new ErrorData(StatusCodes.Status404NotFound, ex.Message));

                builder.Configure((DirectoryNotFoundException ex) =>
                    new ErrorData(StatusCodes.Status404NotFound, ex.Message));

                builder.Configure((FileNotFoundException ex) =>
                    new ErrorData(StatusCodes.Status404NotFound, ex.Message, new { ex.FileName }));
                #endregion

                #region 415
                builder.Configure((ImageFormatException ex) =>
                    new ErrorData(StatusCodes.Status415UnsupportedMediaType, ex.Message, new { ex.CorrectFormats }));
                #endregion

            });

            return services;
        }

        private class ErrorData : IErrorData
        {
            public int StatusCode { get; }
            public string Message { get; }
            public object Data { get; }

            public ErrorData() { }

            public ErrorData(int statusCode, string message = null, object data = null)
            {
                StatusCode = statusCode;
                Message = message;
                Data = data;
            }
        }
    }
}

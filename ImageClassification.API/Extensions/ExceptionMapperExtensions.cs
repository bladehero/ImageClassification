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
                {
                    var _400 = StatusCodes.Status400BadRequest;
                    builder.Configure((EmptyFileException ex) =>
                        new ErrorData(_400, ex.Message));

                    builder.Configure((ArgumentException ex) =>
                        new ErrorData(_400, ex.Message, new { Argument = ex.ParamName }));

                    builder.Configure((ArgumentNullException ex) =>
                        new ErrorData(_400, ex.Message, new { Argument = ex.ParamName }));

                    builder.Configure((IOException ex) =>
                        new ErrorData(_400, ex.Message));
                }
                #endregion

                #region 404
                {
                    var _404 = StatusCodes.Status404NotFound;
                    builder.Configure((NotFoundClassifierException ex) =>
                        new ErrorData(_404, ex.Message));

                    builder.Configure((DirectoryNotFoundException ex) =>
                        new ErrorData(_404, ex.Message));

                    builder.Configure((FileNotFoundException ex) =>
                        new ErrorData(_404, ex.Message, new { ex.FileName }));
                }
                #endregion

                #region 415
                {
                    var _415 = StatusCodes.Status415UnsupportedMediaType;
                    builder.Configure((ImageFormatException ex) =>
                        new ErrorData(_415, ex.Message, new { ex.CorrectFormats }));
                }
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

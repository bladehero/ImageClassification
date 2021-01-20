using ImageClassification.API.Interfaces;
using System;

namespace ImageClassification.API.Services
{
    public class ExceptionMapper : IExceptionMapper
    {
        public IErrorData Map(Exception exception)
        {
            // TODO: Impl
            return default;
        }

        private class InternalErrorData : IErrorData
        {
            public int StatusCode { get; }
            public string Message { get; }
            public object Data { get; }

            public InternalErrorData(int statusCode, string message = null, object data = null)
            {
                StatusCode = statusCode;
                Message = message;
                Data = data;
            }
        }
    }
}

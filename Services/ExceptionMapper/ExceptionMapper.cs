using ExceptionMapper.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace ExceptionMapper
{
    /// <summary>
    /// Mapper service for converting exception into <see cref="IErrorData">IErrorData</see> objects.
    /// </summary>
    internal class ExceptionMapper : IExceptionMapper
    {
        private readonly Builder _builder;

        internal ExceptionMapper()
        {
            _builder = new Builder(new Dictionary<Type, Func<Exception, IErrorData>>());
        }
        internal ExceptionMapper(Action<IExceptionMappingBuilder> configuration) : this()
        {
            configuration.Invoke(_builder);
        }

        public IExceptionMapper Configure<T>(Func<T, IErrorData> factory) where T : Exception
        {
            _builder.Configure(factory);
            return this;
        }
        public IErrorData Map(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var mapping = _builder.Mappings.FirstOrDefault(x => x.Key.Equals(exception.GetType()));

            var error = mapping.Value?.Invoke(exception);

            return error ?? InternalErrorData._default;
        }


        /// <summary>
        /// Mapping builder for providing error data per exception type.
        /// </summary>
        internal class Builder : IExceptionMappingBuilder
        {
            internal IDictionary<Type, Func<Exception, IErrorData>> Mappings { get; }
            internal Builder(IDictionary<Type, Func<Exception, IErrorData>> mappings)
            {
                Mappings = mappings;
            }

            public void Configure<T>(Func<T, IErrorData> factory) where T : Exception
            {
                if (factory is null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }

                var type = typeof(T);
                if (Mappings.ContainsKey(type))
                {
                    throw new ArgumentException($"A mapping with exception type `{type}` already exists in the dictionary.");
                }

                IErrorData basedFactory(Exception exception) => factory((T)exception);
                Mappings.Add(typeof(T), basedFactory);
            }
        }

        /// <summary>
        /// Internal error data representation.
        /// </summary>
        private class InternalErrorData : IErrorData
        {
            internal const string _defaultMessage = "Internal server error";
            internal readonly static InternalErrorData _default = new InternalErrorData(StatusCodes.Status500InternalServerError, _defaultMessage);

            public int StatusCode { get; }
            public string Message { get; }
            public object? Data { get; }

            private InternalErrorData(int statusCode, string message, object? data = null)
            {
                StatusCode = statusCode;
                Message = message;
                Data = data;
            }
        }
    }
}

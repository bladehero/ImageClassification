using System;

namespace ExceptionMapper.Interfaces
{
    public interface IExceptionMappingBuilder
    {
        /// <summary>
        /// Method for adding new exception-to-error mapping.
        /// </summary>
        /// <typeparam name="T">Type of exception which is processed.</typeparam>
        /// <param name="factory">Function delegate which customer provides with mapping <see cref="Exception">Exception</see> into <see cref="IErrorData">IErrorData</see> object.</param>
        void Configure<T>(Func<T, IErrorData> factory) where T : Exception;
    }
}

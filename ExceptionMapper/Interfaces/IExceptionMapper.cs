using System;

#nullable enable

namespace ExceptionMapper.Interfaces
{
    public interface IExceptionMapper
    {
        /// <summary>
        /// Configuring new error per exception type.
        /// </summary>
        /// <typeparam name="T">Type of exception which is processed.</typeparam>
        /// <param name="factory">Function delegate which customer provides with mapping <see cref="Exception">Exception</see> into <see cref="IErrorData">IErrorData</see> object.</param>
        /// <returns>Instance of configured <see cref="IExceptionMapper">IExceptionMapper</see>.</returns>
        IExceptionMapper Configure<T>(Func<T, IErrorData> factory) where T : Exception;

        /// <summary>
        /// Maps <see cref="Exception">Exception</see> into <see cref="IErrorData">IErrorData</see> object.
        /// </summary>
        /// <param name="exception">Exception in application which should be mapped with custom error.</param>
        /// <returns>Returns concrete object of <see cref="IErrorData">IErrorData</see></returns>
        IErrorData Map(Exception exception);
    }
}
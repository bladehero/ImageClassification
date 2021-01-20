using System;

namespace ImageClassification.API.Interfaces
{
    public interface IExceptionMapper
    {
        IErrorData Map(Exception exception);
    }
}
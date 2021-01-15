using ImageClassification.Core.Train.Exceptions;
using System;
using System.IO;

namespace ImageClassification.Core.Train.Common
{
    internal static class ThrowHelper
    {
        internal static void Argument(string message, Exception innerException = null)
        {
            throw new ArgumentException(message, innerException);
        }
        internal static void Argument(string message, string paramName, Exception innerException = null)
        {
            throw new ArgumentException(message, paramName, innerException);
        }
        internal static void ArgumentNull(string parameter, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        internal static void ArgumentOutOfRange(string parameter, object value, string message, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        internal static void SystemEntryNotFound(string path, Exception innerException = null)
        {
            throw new SystemEntryNotFoundException($"Not found system entry path: `{ path }`", innerException) { Path = path };
        }
        internal static void InvalidZipFile(string path, Exception innerException = null)
        {
            throw new InvalidZipFileException($"Invalid zip file by path: `{path}`", innerException) { Path = path };
        }
        internal static void InvalidOperation(string message, Exception innerException = null)
        {
            throw new InvalidOperationException(message, innerException);
        }
        internal static void FileNotFound(string path, Exception innerException = null)
        {
            throw new FileNotFoundException($"Not found file path: `{path}`", innerException);
        }
        internal static void DirectoryNotFound(string path, Exception innerException = null)
        {
            throw new DirectoryNotFoundException($"Not found directory path: `{path}`", innerException);
        }
        internal static void InvalidData(string message, Exception innerException = null)
        {
            throw new InvalidDataException(message, innerException);
        }
        internal static void NullReference(string parameter, Exception innerException = null)
        {
            throw new ArgumentException($"Parameter `{parameter}` must be a valid, non-nullable value!", innerException);
        }

        internal static T Argument<T>(string message, Exception innerException = null)
        {
            throw new ArgumentException(message, innerException);
        }
        internal static T Argument<T>(string message, string paramName, Exception innerException = null)
        {
            throw new ArgumentException(message, paramName, innerException);
        }
        internal static T ArgumentNull<T>(string parameter, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        internal static T ArgumentOutOfRange<T>(string parameter, object value, string message, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        internal static T SystemEntryNotFound<T>(string path, Exception innerException = null)
        {
            throw new SystemEntryNotFoundException($"Not found system entry path: `{ path }`", innerException) { Path = path };
        }
        internal static T InvalidZipFile<T>(string path, Exception innerException = null)
        {
            throw new InvalidZipFileException($"Invalid zip file by path: `{path}`", innerException) { Path = path };
        }
        internal static T InvalidOperation<T>(string message, Exception innerException = null)
        {
            throw new InvalidOperationException(message, innerException);
        }
        internal static T FileNotFound<T>(string path, Exception innerException = null)
        {
            throw new FileNotFoundException($"Not found file path: `{path}`", innerException);
        }
        internal static T DirectoryNotFound<T>(string path, Exception innerException = null)
        {
            throw new DirectoryNotFoundException($"Not found directory path: `{path}`", innerException);
        }
        internal static T InvalidData<T>(string message, Exception innerException = null)
        {
            throw new InvalidDataException(message, innerException);
        }
        internal static T NullReference<T>(string parameter, Exception innerException = null)
        {
            throw new ArgumentException($"Parameter `{parameter}` must be a valid, non-nullable value!", innerException);
        }
    }
}

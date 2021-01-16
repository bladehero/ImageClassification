using ImageClassification.Shared.Exceptions;
using System;
using System.IO;

namespace ImageClassification.Shared.Common
{
    public static class ThrowHelper
    {
        public static void Argument(string message, Exception innerException = null)
        {
            throw new ArgumentException(message, innerException);
        }
        public static void Argument(string message, string paramName, Exception innerException = null)
        {
            throw new ArgumentException(message, paramName, innerException);
        }
        public static void ArgumentNull(string parameter, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        public static void ArgumentOutOfRange(string parameter, object value, string message, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        public static void SystemEntryNotFound(string path, Exception innerException = null)
        {
            throw new SystemEntryNotFoundException($"Not found system entry path: `{ path }`", innerException) { Path = path };
        }
        public static void InvalidZipFile(string path, Exception innerException = null)
        {
            throw new InvalidZipFileException($"Invalid zip file by path: `{path}`", innerException) { Path = path };
        }
        public static void InvalidOperation(string message, Exception innerException = null)
        {
            throw new InvalidOperationException(message, innerException);
        }
        public static void FileNotFound(string path, Exception innerException = null)
        {
            throw new FileNotFoundException($"Not found file path: `{path}`", innerException);
        }
        public static void DirectoryNotFound(string path, Exception innerException = null)
        {
            throw new DirectoryNotFoundException($"Not found directory path: `{path}`", innerException);
        }
        public static void InvalidData(string message, Exception innerException = null)
        {
            throw new InvalidDataException(message, innerException);
        }
        public static void NullReference(string parameter, Exception innerException = null)
        {
            throw new ArgumentException($"Parameter `{parameter}` must be a valid, non-nullable value!", innerException);
        }

        public static T Argument<T>(string message, Exception innerException = null)
        {
            throw new ArgumentException(message, innerException);
        }
        public static T Argument<T>(string message, string paramName, Exception innerException = null)
        {
            throw new ArgumentException(message, paramName, innerException);
        }
        public static T ArgumentNull<T>(string parameter, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        public static T ArgumentOutOfRange<T>(string parameter, object value, string message, Exception innerException = null)
        {
            throw new ArgumentNullException(parameter, innerException);
        }
        public static T SystemEntryNotFound<T>(string path, Exception innerException = null)
        {
            throw new SystemEntryNotFoundException($"Not found system entry path: `{ path }`", innerException) { Path = path };
        }
        public static T InvalidZipFile<T>(string path, Exception innerException = null)
        {
            throw new InvalidZipFileException($"Invalid zip file by path: `{path}`", innerException) { Path = path };
        }
        public static T InvalidOperation<T>(string message, Exception innerException = null)
        {
            throw new InvalidOperationException(message, innerException);
        }
        public static T FileNotFound<T>(string path, Exception innerException = null)
        {
            throw new FileNotFoundException($"Not found file path: `{path}`", innerException);
        }
        public static T DirectoryNotFound<T>(string path, Exception innerException = null)
        {
            throw new DirectoryNotFoundException($"Not found directory path: `{path}`", innerException);
        }
        public static T InvalidData<T>(string message, Exception innerException = null)
        {
            throw new InvalidDataException(message, innerException);
        }
        public static T NullReference<T>(string parameter, Exception innerException = null)
        {
            throw new ArgumentException($"Parameter `{parameter}` must be a valid, non-nullable value!", innerException);
        }
    }
}

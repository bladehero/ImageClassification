namespace ExceptionMapper.Interfaces
{
    public interface IErrorData
    {
        /// <summary>
        /// Custom status code.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// User-friendly message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Related data to error emergence.
        /// </summary>
        public object Data { get; }
    }
}

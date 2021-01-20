namespace ImageClassification.API.Interfaces
{
    public interface IErrorData
    {
        public int StatusCode { get; }
        public string Message { get; }
        public object Data { get; }
    }
}

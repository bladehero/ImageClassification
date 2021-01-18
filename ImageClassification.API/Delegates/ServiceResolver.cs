namespace ImageClassification.API.Delegates
{
    public delegate TService ServiceResolver<TKey, out TService>(TKey key);
}

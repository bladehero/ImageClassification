using Microsoft.ML;

namespace ImageClassification.API.Interfaces
{
    public interface IPredictionEnginePoolService<TData, TPrediction>
        where TData : class
        where TPrediction : class, new()
    {
        bool Add(string classifier);
        bool AddOrUpdate(string classifier);
        void Clear();
        bool ContainsKey(string classifier);
        bool Get(string classifier, out PredictionEngine<TData, TPrediction> engine);
        bool Remove(string key);
        bool Update(string classifier);
    }
}
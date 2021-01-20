using ImageClassification.API.Configurations;
using ImageClassification.API.Exceptions;
using ImageClassification.API.Global;
using ImageClassification.API.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace ImageClassification.API.Services
{
    public class PredictionEnginePoolService<TData, TPrediction> : IPredictionEnginePoolService<TData, TPrediction>
        where TData : class
        where TPrediction : class, new()
    {
        private readonly MLModelOptions _mlModelOptions;
        private readonly ConcurrentDictionary<string, PredictionEngine<TData, TPrediction>> _engines;
        private readonly MLContext _mlContext;

        public PredictionEnginePoolService(IOptions<MLModelOptions> mlOptions, MLContext mlContext)
        {
            _mlModelOptions = mlOptions.Value;
            _engines = new ConcurrentDictionary<string, PredictionEngine<TData, TPrediction>>();
            _mlContext = mlContext;
        }

        public bool Get(string classifier, out PredictionEngine<TData, TPrediction> engine) => _engines.TryGetValue(classifier, out engine);
        public bool Add(string classifier)
        {
            var path = ClassifierPathFindHelper(classifier);

            return AddOrUpdateHelper(path, classifier);
        }
        public bool Update(string classifier)
        {
            var path = ClassifierPathFindHelper(classifier);

            if (!Get(classifier, out PredictionEngine<TData, TPrediction> previous))
            {
                return false;
            }

            return AddOrUpdateHelper(path, classifier, previous);
        }
        public bool AddOrUpdate(string classifier)
        {
            var path = ClassifierPathFindHelper(classifier);
            Get(classifier, out PredictionEngine<TData, TPrediction> previous);
            return AddOrUpdateHelper(path, classifier, previous);
        }
        public bool ContainsKey(string classifier) => _engines.ContainsKey(classifier);
        public void Clear() => _engines.Clear();
        public bool Remove(string key) => _engines.TryRemove(key, out PredictionEngine<TData, TPrediction> _);

        #region Helpers
        private string ClassifierPathFindHelper(string classifier)
        {
            if (string.IsNullOrWhiteSpace(classifier))
            {
                throw new ArgumentException($"'{nameof(classifier)}' cannot be null or whitespace", nameof(classifier));
            }

            var path = Path.Combine(_mlModelOptions.MLModelFilePath, Path.ChangeExtension(classifier, Constants.Extensions.Zip));
            if (!File.Exists(path))
            {
                throw new NotFoundClassifierException($"Classifier `{classifier}` is not found");
            }
            return path;
        }

        private bool AddOrUpdateHelper(string path, string classifier, PredictionEngine<TData, TPrediction> previous = null)
        {
            var trainedModel = _mlContext.Model.Load(path, out DataViewSchema _);
            var engine = _mlContext.Model.CreatePredictionEngine<TData, TPrediction>(trainedModel);

            return previous is null ? _engines.TryAdd(classifier, engine) : _engines.TryUpdate(classifier, engine, previous);
        }
        #endregion
    }
}

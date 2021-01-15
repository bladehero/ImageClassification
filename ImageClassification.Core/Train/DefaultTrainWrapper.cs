using ImageClassification.Core.Train.Common;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static Microsoft.ML.Vision.ImageClassificationTrainer;
using System.Reflection;
using static Microsoft.ML.DataOperationsCatalog;

namespace ImageClassification.Core.Train
{
    public class DefaultTrainWrapper
    {
        #region Private fields
        private Stopwatch _stopwatch;
        private double testFraction = 0.1;
        private readonly MLContext mlContext = new MLContext(seed: 1);
        private Action<TrainProgress> _progress;
        #endregion

        #region Service Properties
        public Stopwatch Stopwatch
        {
            get => _stopwatch;
            private set
            {
                StepCollection.Stopwatch = _stopwatch = value;
            }
        }
        public string Archive { get; private set; }
        public string Folder { get; private set; }

        /// <summary>
        /// Assembly where steps are stored.
        /// </summary>
        public Assembly StepAssembly
        {
            set
            {
                if (value is null)
                {
                    ThrowHelper.NullReference(nameof(StepAssembly));
                }

                StepCollection.StepAssembly = value;
            }
        }

        /// <summary>
        /// Attribute for filtering steps while loading from certain <see cref="Assembly">Assembly</see>.
        /// </summary>
        public Type StepCaptureAttributeType
        {
            set
            {
                if (value is null)
                {
                    ThrowHelper.NullReference(nameof(StepCaptureAttributeType));
                }

                StepCollection.StepCaptureAttributeType = value;
            }
        }
        internal StepCollection StepCollection { get; set; }
        #endregion

        #region Internal Settings
        public bool MeasureTime { get; set; } = false;
        public bool DeleteArchiveAfterTrainning { get; set; } = false;
        public bool UseEvaluation { get; set; } = true;
        #endregion

        #region Trainning Options

        public Options Options { get; }
        public double TestFraction
        {
            get => testFraction;
            set
            {
                if (value <= 0 || value >= 1)
                {
                    ThrowHelper.ArgumentOutOfRange(nameof(TestFraction), value, "Test fraction should be between 0 and 1");
                }
                testFraction = value;
            }
        }
        #endregion

        #region Events
        public event Action<ImageClassificationMetrics> ImageMetricsUpdated;
        public event Action<MulticlassClassificationMetrics> MulticlassMetricsUpdated;
        public event Action<TrainProgress> ProgressChanged
        {
            add
            {
                if (value != null)
                {
                    _progress += value;
                    StepCollection.TrainLog += value;
                }
            }
            remove
            {
                if (value != null)
                {
                    _progress -= value;
                    StepCollection.TrainLog -= value;
                }
            }
        }

        public event EventHandler<LoggingEventArgs> Log
        {
            add
            {
                mlContext.Log += value;
            }
            remove
            {
                mlContext.Log -= value;
            }
        }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor for trainning wrapper.
        /// </summary>
        /// <param name="path">Path to archive or folder with images.</param>
        public DefaultTrainWrapper(string path)
        {
            StepCollection = new StepCollection();
            Options = new Options()
            {
                FeatureColumnName = "Image",
                LabelColumnName = "LabelAsKey",
                Arch = Architecture.InceptionV3,
                Epoch = 200,
                BatchSize = 10,
                LearningRate = 0.01f,
                MetricsCallback = (metrics) => ImageMetricsUpdated?.Invoke(metrics)
            };

            if (File.Exists(path) == false && Directory.Exists(path) == false)
            {
                ThrowHelper.SystemEntryNotFound(path);
            }

            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                Folder = path;
            }
            else
            {
                if (ZipFileValidationHelper.IsValid(path))
                {
                    Archive = path;
                    Folder = Path.Join(Path.GetDirectoryName(path),
                                       Path.GetFileNameWithoutExtension(path));
                }
                else
                {
                    ThrowHelper.InvalidZipFile(path);
                }
            }
        }
        #endregion

        /// <summary>
        /// Trains neurone network and saves it.
        /// </summary>
        /// <param name="stream">Stream where is saved trained model.</param>
        /// <returns>Boolean flag of success.</returns>
        public async Task<bool> TrainAsync(Stream stream)
        {
            var totalElapsed = new TimeSpan();

            if (MeasureTime)
            {
                Stopwatch = Stopwatch.StartNew();
            }

            if (File.Exists(Archive))
            {
                var unarchiving = StepCollection.GetStep<(string, string), Task>(StepName.Unarchiving);
                await unarchiving.Execute((Archive, Folder));
                totalElapsed += Stopwatch.RestartPull();
            }

            var prepareDataSet = StepCollection.GetStep<(string, MLContext), IDataView>(StepName.PreparingDataSet);
            var shuffledDataSet = prepareDataSet.Execute((Folder, mlContext));
            totalElapsed += Stopwatch.RestartPull();

            var loadImages = StepCollection.GetStep<(string, MLContext, IDataView), IDataView>(StepName.LoadingImages);
            var inMemoryDataSet = loadImages.Execute((Folder, mlContext, shuffledDataSet));
            totalElapsed += Stopwatch.RestartPull();

            var splitData = StepCollection.GetStep<(MLContext, IDataView, double), TrainTestData>(StepName.SplittingData);
            var trainTestData = splitData.Execute((mlContext, inMemoryDataSet, TestFraction));
            totalElapsed += Stopwatch.RestartPull();

            Options.ValidationSet = trainTestData.TestSet;
            var define = StepCollection.GetStep<(MLContext, Options), IEstimator<ITransformer>>(StepName.DefiningModel);
            var pipeline = define.Execute((mlContext, Options));
            totalElapsed += Stopwatch.RestartPull();

            var train = StepCollection.GetStep<(IEstimator<ITransformer>, IDataView), ITransformer>(StepName.Trainning);
            var trainedModel = train.Execute((pipeline, trainTestData.TrainSet));
            totalElapsed += Stopwatch.RestartPull();

            if (UseEvaluation)
            {
                var evaluate = StepCollection.GetStep<(MLContext, IDataView, ITransformer), MulticlassClassificationMetrics>(StepName.EvaluatingModel);
                var metrics = evaluate.Execute((mlContext, trainTestData.TestSet, trainedModel));
                MulticlassMetricsUpdated?.Invoke(metrics);
                totalElapsed += Stopwatch.RestartPull();
            }

            var save = StepCollection.GetStep<(MLContext, ITransformer, DataViewSchema, Stream), bool>(StepName.SavingModel);
            var success = save.Execute((mlContext, trainedModel, trainTestData.TrainSet.Schema, stream));
            totalElapsed += Stopwatch.RestartPull();

            if (MeasureTime)
            {
                Stopwatch.Stop();
                _progress?.Invoke(new TrainProgress
                {
                    Message = $"Total training took: {totalElapsed}",
                    Status = StepStatus.Finished
                });
                Stopwatch = null;
            }

            return success;
        }

        /// <summary>
        /// Trains neurone network and saves it.
        /// </summary>
        /// <param name="destination">Path to file where is saved trained model.</param>
        /// <returns>Boolean flag of success.</returns>
        public async Task<bool> TrainAsync(string destination, FileMode fileMode = FileMode.CreateNew)
        {
            using var stream = new FileStream(destination, fileMode);
            return await TrainAsync(stream);
        }
    }
}

using ImageClassification.Core.Train.Common;
using ImageClassification.Core.Train.DataModels;
using ImageClassification.Core.Train.Exceptions;
using ImageClassification.Shared;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.ML.DataOperationsCatalog;
using static Microsoft.ML.Transforms.ValueToKeyMappingEstimator;
using static Microsoft.ML.Vision.ImageClassificationTrainer;

namespace ImageClassification.Core.Train
{
    public class TrainWrapper
    {
        #region Private fields
        private Stopwatch stopwatch;
        private MLContext mlContext = new MLContext(seed: 1);
        #endregion

        #region Service Properties
        public string Archive { get; private set; }
        public string Folder { get; private set; }
        public TrainStepStatus TrainStepStatus { get; set; } = TrainStepStatus.Initialization;
        #endregion

        #region Internal Settings
        public bool MeasureTime { get; set; } = false;
        public bool DeleteArchiveAfterTrainning { get; set; } = false;
        public bool UseEvaluation { get; set; } = true;
        #endregion

        #region Trainning Options
        public double TestFraction { get; set; } = 0.1;
        public string FeatureColumnName { get; set; } = "Image";
        public string LabelColumnName { get; set; } = "LabelAsKey";
        public Architecture Architecture { get; set; } = Architecture.InceptionV3;
        public int Epoch { get; set; } = 200;
        public int BatchSize { get; set; } = 10;
        public float LearningRate { get; set; } = 0.01f;
        #endregion

        #region Events
        public event Action<ImageClassificationMetrics> ImageMetricsUpdated;
        public event Action<MulticlassClassificationMetrics> MulticlassMetricsUpdated;
        public event Action<TrainProgress> StepChanged;

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
        public TrainWrapper(string path)
        {
            if (File.Exists(path) == false && Directory.Exists(path) == false)
            {
                throw new SystemEntryNotFoundException { Path = path };
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
                    throw new InvalidZipFileException { Path = path };
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<bool> TryTrainAsync(Stream stream)
        {
            if (MeasureTime)
            {
                stopwatch = new Stopwatch();
            }

            if (File.Exists(Archive))
            {
                await Unarchiving(Archive, Folder);
            }

            var shuffledDataSet = PrepareDataSet(Folder, mlContext);

            var inMemoryDataSet = LoadImages(Folder, shuffledDataSet);

            var trainTestData = SplitData(inMemoryDataSet);

            var pipeline = Define(trainTestData.TestSet);

            var trainedModel = Train(pipeline, trainTestData.TrainSet);

            if (UseEvaluation)
            {
                var metrics = Evaluate(trainTestData.TestSet, trainedModel);
                MulticlassMetricsUpdated?.Invoke(metrics);
            }

            Save(trainedModel, trainTestData.TrainSet.Schema, stream);

            if (MeasureTime)
            {
                stopwatch = null;
            }

            return true;
        }

        /// <summary>
        /// 1. Prepare ML Context and unarchive source
        /// </summary>
        /// <param name="sourceDirectory">Path to directory where images will be stored.</param>
        /// <returns>Returns a task of processing.</returns>
        private async Task Unarchiving(string archive, string sourceDirectory)
        {
            TrainStepStatus = TrainStepStatus.Unarchiving;
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Started unarchiving process for archive: `{Archive}`"
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            if (Directory.Exists(sourceDirectory))
            {
                Directory.Delete(sourceDirectory, true);
            }
            Directory.CreateDirectory(sourceDirectory);
            await Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(archive, sourceDirectory);
            });

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Finished unarchiving",
                Elapsed = stopwatch?.Elapsed
            });
        }

        /// <summary>
        /// 2. Image-set using IDataView and shuffle for better balance
        /// </summary>
        /// <param name="sourceDirectory">Path to directory where images will be stored.</param>
        /// <param name="mlContext">Context of ML.NET for training model.</param>
        /// <returns>Shuffled full image file paths data set.</returns>
        private IDataView PrepareDataSet(string sourceDirectory, MLContext mlContext)
        {
            TrainStepStatus = TrainStepStatus.PreparingDataSet;
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Preparing data set"
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            var images = FileUtils.LoadImagesFromDirectory(sourceDirectory, true)
                                  .Select(x => new ImageData(x.ImagePath, x.Label));
            var fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
            var shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Data set is prepared",
                Elapsed = stopwatch?.Elapsed
            });

            return shuffledFullImageFilePathsDataset;
        }

        /// <summary>
        /// 3. Load Images with in-memory type within the IDataView and Transform Labels to Keys (Categorical)
        /// </summary>
        /// <param name="sourceDirectory">Path to directory where images will be stored.</param>
        /// <param name="dataSet">Shuffled full image file paths data set.</param>
        /// <returns>Shuffled images in-memory data set.</returns>
        private IDataView LoadImages(string sourceDirectory, IDataView dataSet)
        {
            TrainStepStatus = TrainStepStatus.LoadingImages;
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Loading images in memory"
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            var shuffledFullImagesDataset =
                    mlContext.Transforms
                             .Conversion
                             .MapValueToKey("LabelAsKey",
                                            "Label",
                                            keyOrdinality: KeyOrdinality.ByValue)
                             .Append(mlContext.Transforms.LoadRawImageBytes("Image",
                                                                            sourceDirectory,
                                                                            "ImagePath"))
                             .Fit(dataSet)
                             .Transform(dataSet);

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Loading images in memory has been finished",
                Elapsed = stopwatch?.Elapsed
            });

            return shuffledFullImagesDataset;
        }

        /// <summary>
        /// 4. Split the data by formula as default `90:10` into train and test sets, train and evaluate.
        /// Change <see cref="TestFraction">TestFraction</see> to change the ratio.
        /// </summary>
        /// <param name="dataSet">Shuffled images in-memory data set.</param>
        /// <returns>Train and test data.</returns>
        private TrainTestData SplitData(IDataView dataSet)
        {
            TrainStepStatus = TrainStepStatus.SplittingData;
            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            var trainTestData = mlContext.Data.TrainTestSplit(dataSet, TestFraction);

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Data splitted by formula Train-set/Test-set {100 - TestFraction * 100}/{TestFraction * 100}",
                Elapsed = stopwatch?.Elapsed
            });

            return trainTestData;
        }

        /// <summary>
        /// 5. Define the model's training pipeline by using explicit hyper-parameters
        /// </summary>
        /// <param name="testDataSet">Test data set.</param>
        /// <returns>Pipeline for trainning process.</returns>
        private IEstimator<ITransformer> Define(IDataView testDataSet)
        {
            TrainStepStatus = TrainStepStatus.DefiningModel;
            var options = new Options()
            {
                FeatureColumnName = FeatureColumnName,
                LabelColumnName = LabelColumnName,
                Arch = Architecture,
                Epoch = Epoch,
                BatchSize = BatchSize,
                LearningRate = LearningRate,
                MetricsCallback = (metrics) => ImageMetricsUpdated?.Invoke(metrics),
                ValidationSet = testDataSet
            };
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Model will be defined with options:{Environment.NewLine}" +
                $"Epoch={options.Epoch}{Environment.NewLine}" +
                $"Architecture={options.Arch}{Environment.NewLine}" +
                $"BatchSize={options.BatchSize}{Environment.NewLine}" +
                $"LearningRate={options.LearningRate}{Environment.NewLine}" +
                $"FeatureColumnName={options.FeatureColumnName}{Environment.NewLine}" +
                $"LabelColumnName={options.FeatureColumnName}{Environment.NewLine}"
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            var pipeline =
                mlContext.MulticlassClassification
                         .Trainers
                         .ImageClassification(options)
                         .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel",
                                                                               "PredictedLabel"));

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Model is defined",
                Elapsed = stopwatch?.Elapsed
            });

            return pipeline;
        }

        /// <summary>
        /// 6. Train/create the ML model
        /// </summary>
        /// <param name="pipeline">Pipeline for trainning process.</param>
        /// <param name="trainDataSet">Train data set.</param>
        /// <returns>Trained model.</returns>
        private ITransformer Train(IEstimator<ITransformer> pipeline, IDataView trainDataSet)
        {
            TrainStepStatus = TrainStepStatus.Trainning;
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Started training process..."
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            ITransformer trainedModel = pipeline.Fit(trainDataSet);

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Finished training process",
                Elapsed = stopwatch?.Elapsed
            });

            return trainedModel;
        }

        /// <summary>
        /// 7. Get the quality metrics (accuracy, etc.)
        /// </summary>
        /// <param name="testDataSet">Test data set.</param>
        /// <param name="trainedModel">Trained model.</param>
        private MulticlassClassificationMetrics Evaluate(IDataView testDataSet, ITransformer trainedModel)
        {
            TrainStepStatus = TrainStepStatus.EvaluatingModel;
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Started evaluating model..."
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            var predictionsDataView = trainedModel.Transform(testDataSet);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName: "LabelAsKey", predictedLabelColumnName: "PredictedLabel");

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Finished evaluating model",
                Elapsed = stopwatch?.Elapsed
            });

            return metrics;
        }

        /// <summary>
        /// 8. Save the model (ML.NET .zip model file and TensorFlow .pb model file)
        /// </summary>
        /// <param name="trainedModel">Trained model.</param>
        /// <param name="trainSchema">Schema of train data set.</param>
        /// <param name="stream">A writeable, seekable stream to save to.</param>
        public void Save(ITransformer trainedModel, DataViewSchema trainSchema, Stream stream)
        {
            TrainStepStatus = TrainStepStatus.SavingModel;
            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Started saving model..."
            });

            if (MeasureTime)
            {
                stopwatch.Restart();
            }

            mlContext.Model.Save(trainedModel, trainSchema, stream);

            if (MeasureTime)
            {
                stopwatch.Stop();
            }

            StepChanged?.Invoke(new TrainProgress
            {
                Current = TrainStepStatus,
                Message = $"Finished saving model",
                Elapsed = stopwatch?.Elapsed
            });
        }
    }
}

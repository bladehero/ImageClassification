using ImageClassification.API.Interfaces;
using ImageClassification.Core.Train;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ImageClassification.API.Services
{
    public class TrainWrapperProxy : ITrainProxyWrapper, ITrainProxy, ITrainWrapper
    {
        private DefaultTrainWrapper _defaultTrainWrapper = null;

        public string Archive => _defaultTrainWrapper.Archive;
        public bool DeleteArchiveAfterTrainning
        {
            get => _defaultTrainWrapper.DeleteArchiveAfterTrainning;
            set => _defaultTrainWrapper.DeleteArchiveAfterTrainning = value;
        }
        public string Folder => _defaultTrainWrapper.Folder;
        public bool MeasureTime
        {
            get => _defaultTrainWrapper.MeasureTime;
            set => _defaultTrainWrapper.MeasureTime = value;
        }
        public ImageClassificationTrainer.Options Options => _defaultTrainWrapper.Options;
        public Assembly StepAssembly { set => _defaultTrainWrapper.StepAssembly = value; }
        public Type StepCaptureAttributeType { set => _defaultTrainWrapper.StepCaptureAttributeType = value; }
        public Stopwatch Stopwatch => _defaultTrainWrapper.Stopwatch;
        public double TestFraction
        {
            get => _defaultTrainWrapper.TestFraction;
            set => _defaultTrainWrapper.TestFraction = value;
        }
        public bool UseEvaluation
        {
            get => _defaultTrainWrapper.UseEvaluation;
            set => _defaultTrainWrapper.UseEvaluation = value;
        }
        public string Path { set => _defaultTrainWrapper = new DefaultTrainWrapper(value); }

        public event Action<ImageClassificationTrainer.ImageClassificationMetrics> ImageMetricsUpdated
        {
            add
            {
                _defaultTrainWrapper.ImageMetricsUpdated += value;
            }
            remove
            {
                _defaultTrainWrapper.ImageMetricsUpdated -= value;
            }
        }
        public event EventHandler<LoggingEventArgs> Log
        {
            add
            {
                _defaultTrainWrapper.Log += value;
            }
            remove
            {
                _defaultTrainWrapper.Log -= value;
            }
        }
        public event Action<MulticlassClassificationMetrics> MulticlassMetricsUpdated
        {
            add
            {
                _defaultTrainWrapper.MulticlassMetricsUpdated += value;
            }
            remove
            {
                _defaultTrainWrapper.MulticlassMetricsUpdated -= value;
            }
        }
        public event Action<TrainProgress> ProgressChanged
        {
            add
            {
                _defaultTrainWrapper.ProgressChanged += value;
            }
            remove
            {
                _defaultTrainWrapper.ProgressChanged -= value;
            }
        }

        public Task<IEnumerable<string>> TrainAsync(Stream stream)
        {
            return _defaultTrainWrapper.TrainAsync(stream);
        }

        public Task<IEnumerable<string>> TrainAsync(string destination, FileMode fileMode = FileMode.CreateNew)
        {
            return _defaultTrainWrapper.TrainAsync(destination, fileMode);
        }
    }
}

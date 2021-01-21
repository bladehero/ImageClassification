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

namespace ImageClassification.Core.Train
{
    public interface ITrainWrapper
    {
        string Archive { get; }
        bool DeleteArchiveAfterTrainning { get; set; }
        string Folder { get; }
        bool MeasureTime { get; set; }
        ImageClassificationTrainer.Options Options { get; }
        Assembly StepAssembly { set; }
        Type StepCaptureAttributeType { set; }
        Stopwatch Stopwatch { get; }
        double TestFraction { get; set; }
        bool UseEvaluation { get; set; }

        event Action<ImageClassificationTrainer.ImageClassificationMetrics> ImageMetricsUpdated;
        event EventHandler<LoggingEventArgs> Log;
        event Action<MulticlassClassificationMetrics> MulticlassMetricsUpdated;
        event Action<TrainProgress> ProgressChanged;

        Task<IEnumerable<string>> TrainAsync(Stream stream);
        Task<IEnumerable<string>> TrainAsync(string destination, FileMode fileMode = FileMode.CreateNew);
    }
}
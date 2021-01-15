using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Common;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class UnarchivingStep : BaseStep, ITrainStep<(string Archive, string SourceDirectory), Task>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName => StepName.Unarchiving;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Prepare ML Context and unarchive source.
        /// </summary>
        /// <remarks>
        /// First (1) step as default. Optional.
        /// </remarks>
        /// <param name="data">Paths to archive where images are stored and to directory where they will be unarchived.</param>
        /// <returns>Returns a task of processing.</returns>
        public async Task Execute((string Archive, string SourceDirectory) data)
        {
            (string archive, string source) = data;

            Log?.Invoke(GenerateStarted($"Started unarchiving process for archive: `{archive}`"));

            #region Validation
            if (string.IsNullOrWhiteSpace(archive))
            {
                ThrowHelper.Argument("Path to archive cannot be null", nameof(archive));
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                ThrowHelper.Argument("Path to source cannot be null", nameof(source));
            }


            if (File.Exists(archive) == false)
            {
                ThrowHelper.FileNotFound(archive);
            }

            if (!File.GetAttributes(archive).HasFlag(FileAttributes.Archive))
            {
                ThrowHelper.InvalidData($"Archive is not valid: {archive}");
            }
            #endregion

            #region Action
            if (Directory.Exists(source))
            {
                Directory.Delete(source, true);
            }
            Directory.CreateDirectory(source);
            await Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(archive, source);
            });
            #endregion

            Log?.Invoke(GenerateFinished($"Finished unarchiving"));
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((string, string))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                return ThrowHelper.Argument<object>("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

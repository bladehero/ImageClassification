using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using ImageClassification.Shared.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ImageClassification.Core.Train
{
    /// <summary>
    /// Can be inherited for implementation with custom <see cref="StepFilter">StepFilter</see>.
    /// </summary>
    internal class StepCollection : IStepCollection, IReadOnlyCollection<ITrainStep>, IEnumerable<ITrainStep>
    {
        #region Service Fields
        private readonly Type _interfaceType = typeof(ITrainStep);
        private Type stepCaptureAttributeType;
        #endregion

        #region Fields for Consuming 
        private IReadOnlyCollection<ITrainStep> trainSteps;
        private Stopwatch _stopwatch;
        private Assembly stepAssembly;
        #endregion

        #region Properties
        /// <summary>
        /// Assembly where steps are stored.
        /// </summary>
        /// <remarks>
        /// As default selected by a method <see cref="Assembly.GetExecutingAssembly">Assembly.GetExecutingAssembly</see>.
        /// </remarks>
        public virtual Assembly StepAssembly
        {
            get => stepAssembly;
            set
            {
                stepAssembly = value;
                LoadTrainSteps();
            }
        }

        /// <summary>
        /// Attribute for filtering steps while loading from certain <see cref="Assembly">Assembly</see>.
        /// </summary>
        /// <remarks>
        /// As default selected internal <see cref="DefaultStepCaptureAttribute">DefaultStepCaptureAttribute</see>.
        /// </remarks>
        public virtual Type StepCaptureAttributeType
        {
            get => stepCaptureAttributeType;
            set
            {
                stepCaptureAttributeType = value;
                LoadTrainSteps();
            }
        }

        /// <summary>
        /// Predicate that allows to override filtering for getting steps by different strategies.
        /// </summary>
        protected virtual Func<ITrainStep, bool> StepFilter { get; } = null;
        public Stopwatch Stopwatch
        {
            get
            {
                return _stopwatch;
            }
            set
            {
                _stopwatch = value;
                foreach (var trainStep in trainSteps)
                {
                    trainStep.Stopwatch = value;
                }
            }
        }
        public event Action<TrainProgress> TrainLog
        {
            add
            {
                if (value != null)
                {
                    foreach (var trainStep in trainSteps)
                    {
                        trainStep.Log += value;
                    }
                }
            }
            remove
            {
                if (value != null)
                {
                    foreach (var trainStep in trainSteps)
                    {
                        trainStep.Log -= value;
                    }
                }
            }
        }
        #endregion

        public StepCollection()
        {
            stepAssembly = Assembly.GetExecutingAssembly();
            stepCaptureAttributeType = typeof(DefaultStepCaptureAttribute);
            LoadTrainSteps();
        }

        /// <summary>
        /// Gets first possible match from the collection of train steps.
        /// </summary>
        /// <param name="stepName">Step name.</param>
        /// <returns>Train step interface.</returns>
        public ITrainStep GetStep(StepName stepName)
        {
            try
            {
                return trainSteps.First(x => x.StepName == stepName && StepFilter?.Invoke(x) != false);
            }
            catch (InvalidOperationException)
            {
                return ThrowHelper.Argument<ITrainStep>($"There is no step with the name: {stepName}", nameof(stepName));
            }
        }

        /// <summary>
        /// Gets first possible match from the collection of train steps.
        /// </summary>
        /// <typeparam name="D">Data type for step.</typeparam>
        /// <typeparam name="TResult">Resolution type for step.</typeparam>
        /// <param name="stepName">Step name.</param>
        /// <returns>Train step interface.</returns>
        public ITrainStep<D, TResult> GetStep<D, TResult>(StepName stepName)
        {
            var step = GetStep(stepName);
            try
            {
                var casted = (ITrainStep<D, TResult>)step;
                return casted;
            }
            catch (InvalidCastException ex)
            {
                return ThrowHelper.InvalidOperation<ITrainStep<D, TResult>>(
                    $"Provided incorrect generic parameters " +
                    $"with data type: `{typeof(D).FullName}` " +
                    $"and result type: `{typeof(TResult).FullName}`, " +
                    $"for mapping of type {step.GetType().FullName}!", ex
                );
            }
        }

        /// <summary>
        /// Count of train steps
        /// </summary>
        public int Count => trainSteps.Count;

        /// <summary>
        /// Gets enumerator of train steps.
        /// </summary>
        /// <returns>Train step's enumerator.</returns>
        public IEnumerator<ITrainStep> GetEnumerator() => trainSteps.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Helpers
        private void LoadTrainSteps()
        {
            var types = stepAssembly.DefinedTypes
                                    .Where(x => x.HasCustomAttribute(StepCaptureAttributeType)
                                             && x.ImplementsInterface(_interfaceType));

            trainSteps = types.Select(x => Activator.CreateInstance(x) as ITrainStep)
                              .Where(x => x != null)
                              .ToList();

            if (trainSteps?.Count == 0)
            {
                ThrowHelper.InvalidOperation("Sequence contains no elements");
            }
        }
        #endregion
    }
}

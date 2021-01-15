using System;
using System.Diagnostics;

namespace ImageClassification.Core.Train.Attributes
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultStepCaptureAttribute : Attribute
    {
        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}

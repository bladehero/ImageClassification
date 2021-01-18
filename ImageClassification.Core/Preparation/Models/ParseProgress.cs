namespace ImageClassification.Core.Preparation.Models
{
    public class ParseProgress
    {
        public int EstimatedCount { get; set; }
        public int CurrentCount { get; set; }
        public float EstimatedProgress => (float)CurrentCount / EstimatedCount;

        /// <summary>
        /// Estimated percentage, if the current count greater then estimated - returns 100% as default.
        /// </summary>
        public float EstimatedPercentage
        {
            get
            {
                var progress = EstimatedProgress;
                if (progress < 1)
                {
                    return progress * 100;
                }
                return 100;
            }
        }
    }
}

using System;

namespace Esfa.Ofsted.Inspection.Types
{
    /// <summary>
    /// Ofsted Inspection Details
    /// </summary>
    public class OfstedInspection
    {
        /// <summary>
        /// Website of provider
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// ukprn of provider
        /// </summary>
        public int Ukprn { get; set; }
        /// <summary>
        /// Date of Report Publication
        /// </summary>
        public DateTime DatePublished { get; set; }
        /// <summary>
        /// Overall Effectiveness Status
        /// </summary>
        public OverallEffectiveness OverallEffectiveness { get; set; }
    }
}

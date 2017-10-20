using System;

namespace Esfa.Ofsted.Inspection.Types
{
   /// <summary>
   /// Details on line errors in processing the ofsted details
   /// </summary>
   [Serializable]
   public  class InspectionError
    {
        /// <summary>
        /// Website text
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// ukprn of provider
        /// </summary>
        public string Ukprn { get; set; }
        /// <summary>
        /// Date of Report Publication text
        /// </summary>
        public string DatePublished { get; set; }
        /// <summary>
        /// Overall Effectiveness Details
        /// </summary>
        public string OverallEffectiveness { get; set; }
        /// <summary>
        /// Line in originating document where error occurred
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Message about error
        /// </summary>
        public string Message => "One or more items do not have a valid value";       
    }
}

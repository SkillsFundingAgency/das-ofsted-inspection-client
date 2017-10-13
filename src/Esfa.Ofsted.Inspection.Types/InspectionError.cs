namespace Esfa.Ofsted.Inspection.Types
{
   /// <summary>
   /// Details on line errors in processing the ofsted details (line = 0 is a file error)
   /// </summary>
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
        public string Message { get; set; }


    }
}
